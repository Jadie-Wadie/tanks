// Packages
const {
	MongoClient,
	ObjectID
} = require('mongodb');

// Modules
var {
	Server
} = require('./modules/server');

// Variables
const subdomain = 'tanks';
const port = 3003;

if (subdomain == null) throw new TypeError('subdomain cannot be null');
if (port == null) throw new TypeError('port cannot be null');

// Setup
var server = new Server(subdomain, port, '/etc/letsencrypt/live/roundsquare.site');
['./public', './public/html', '../WebGL/Build'].forEach(v => server.useStatic(v));

var router = server.start();

var secret = require('./json/secret.json');
var url = `mongodb://${secret.mongo.user}:${secret.mongo.password}@${secret.mongo.url}:${secret.mongo.port}`;

var client = null;
var tanks = null;

console.log(`Name: ${`'${server.subdomain}'`.brightGreen}`);
console.log(`Port: ${server.port.toString().brightYellow}`);
console.log(`Test: ${server.local.toString().brightYellow}`);

(async function () {
	client = await MongoClient.connect(url, {
		useUnifiedTopology: true
	});

	tanks = client.db('roundsquare').collection('tanks');

	router.get('/readScores', async function (req, res) {
		console.log(`*:*`.green);
		res.send(JSON.stringify({
			scores: await tanks.find({}).toArray()
		}));
	});

	router.post('/writeScore', async function (req, res) {
		var name = req.body.name;
		var score = parseInt(req.body.score);

		if (!(/^[A-Z|\d]{1,3}$/.test(name))) return res.send(`name cannot be ${req.body.name}`);
		if (isNaN(score)) return res.send(`score cannot be ${req.body.score}`);

		await tanks.insertOne({
			name,
			score
		}).then(out => {
			console.log(`${name}:${score}`.green);
			res.send({
				name,
				score
			});
		}).catch(err => {
			console.log(`${name}:${score}`.red);
			res.send(err);
		});
	});

	router.get('/deleteScores', async function (req, res) {
		console.log(`*:*`.red);
		await tanks.deleteMany({});
		res.send(JSON.stringify({
			scores: await tanks.find({}).toArray()
		}));
	});
})();