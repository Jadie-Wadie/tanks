// Packages
require('colors');

var fs = require('fs');
var path = require('path');

var http = require('http');
var https = require('https');

var express = require('express');
var subdomain = require('express-subdomain');

var bodyParser = require('body-parser');

var passport = require('passport');
var passportStrategy = require('passport-local').Strategy;

var expressSession = require('express-session');
var memoryStore = require('memorystore')(expressSession);

var socket = require('socket.io');

var cors = require("cors");

// Local Server
class LocalServer {
	constructor(port, ...args) {
		this.port = port;
		this.statics = [];

		this.app = express();
		this.createServer(args);
	}

	createServer() {
		this.server = http.createServer(this.app);
	}

	createRouter() {
		this.router = express.Router();

		this.router.use(
			bodyParser.urlencoded({
				extended: false
			})
		);
		this.router.use(bodyParser.json());

		this.app.use(cors());
		this.app.use('/', this.router);

		this.statics.map(name => this.router.use(express.static(path.join(__dirname, '..', name))));
	}

	useStatic(path) {
		this.statics.push(path);
	}

	usePassport(kick, auth, user, folder = '../private') {
		this.passport = require('passport');

		this.app.set('trust proxy', 1);
		this.app.use(
			expressSession({
				cookie: {
					maxAge: 15000
				},

				name: `${this.subdomain}cookie`,
				secret: 'cookiesecret',
				resave: false,
				saveUninitialized: false,

				store: new memoryStore({
					checkPeriod: 86400000
				})
			})
		);

		this.passport.use(
			new passportStrategy(
				{
					usernameField: 'username',
					passwordField: 'password'
				},
				auth
			)
		);

		this.passport.serializeUser(function(user, cb) {
			cb(null, user.id);
		});

		this.passport.deserializeUser(user);

		this.passport.isAuthenticated = function(request, response, next) {
			if (request.isAuthenticated()) {
				return next();
			} else {
				response.redirect(kick);
			}
		};

		this.passport.isAuthenticatedAPI = function(request, response, next) {
			if (request.isAuthenticated()) {
				return next();
			} else {
				response.send('AJAX not Authenticated');
			}
		};

		this.passport.folder = folder;

		this.app.use(this.passport.initialize());
		this.app.use(this.passport.session());

		return this.passport;
	}

	useSocket() {
		this.socket = socket.listen(this.server);
		return this.socket;
	}

	start() {
		this.createRouter();

		if (this.passport != undefined) {
			this.router.use(
				'/private/',
				this.passport.isAuthenticated,
				function(req, res) {
					res.sendFile(
						path.join(__dirname, this.passport.folder, req.path)
					);
				}.bind(this)
			);
		}

		if (this.local) {
			this.app.use((req, res) =>
				res.end(
					`Error occured while trying find: ${req.headers.host}${req.url}`
				)
			);
		} else {
			this.app.use((req, res) =>
				res.redirect('https://www.roundsquare.site/?code=404')
			);
		}

		this.server.listen(this.port);
		return this.router;
	}
}

class Server extends LocalServer {
	constructor(subdomain, port, certPath) {
		super(port, certPath);
		this.subdomain = subdomain;
	}

	createServer(certPath) {
		this.loadCerts(certPath);
		this.server = this.local
			? http.createServer(this.app)
			: https.createServer(this.credentials, this.app);
	}

	createRouter() {
		this.router = express.Router();

		this.router.use(
			bodyParser.urlencoded({
				extended: false
			})
		);
		this.router.use(bodyParser.json());
		
		this.app.use(cors());
		if (!this.local) {
			this.app.use(subdomain(this.subdomain, this.router));
		} else {
			this.app.use('/', this.router);
		}

		this.statics.map(name => this.router.use(express.static(path.join(__dirname, '..', name))));
	}

	loadCerts(path) {
		this.local = false;

		try {
			this.credentials = {
				key: fs.readFileSync(path + '/privkey.pem', 'utf8'),
				cert: fs.readFileSync(path + '/cert.pem', 'utf8'),
				ca: fs.readFileSync(path + '/chain.pem', 'utf8')
			};
		} catch (e) {
			this.local = true;
		}
	}
}

// Export
module.exports = {
	Server,
	LocalServer
};
