// Variables
var objects;

const start = Date.now();
var runtime = 0;

const speed = 25;
const color = {
	r: 255,
	g: 255,
	b: 255
};

// Main
$(window).on('load', function () {
	objects = $(".logo").map((i, v) => {
		return {
			canvas: v,
			jobj: $(v),
			context: v.getContext('2d'),
			size: 0,
			stroke: 0
		};
	});

	$(window).resize(resize);
	resize();

	update();
});

// Update
function update() {
	requestAnimationFrame(update);
	runtime = (Date.now() - start) / 1000;

	for (let i = 0; i < objects.length; i++) {
		var obj = objects[i];
		reset(obj);

		for (let i = 0; i < 9; i++) {
			strokeRect(obj.context, obj.canvas.width / 2, obj.canvas.height / 2, obj.size, obj.size, `rgba(${color.r}, ${color.g}, ${color.b}, ${0.5})`, obj.stroke, runtime * speed + (i * 10));
		}
	}
}

// Resize
function resize() {
	for (let i = 0; i < objects.length; i++) {
		var obj = objects[i];

		obj.canvas.width = obj.jobj.width();
		obj.canvas.height = obj.jobj.height();

		obj.size = Math.min(obj.canvas.width, obj.canvas.height) * 0.65;
		obj.stroke = Math.min(obj.canvas.width, obj.canvas.height) * 0.02;
	}
}

// Reset
function reset(obj) {
	obj.context.save();

	obj.context.setTransform(1, 0, 0, 1, 0, 0);
	obj.context.clearRect(0, 0, obj.canvas.width, obj.canvas.height);

	obj.context.restore();
}

// Rect
function strokeRect(c, x, y, w, h, col, wid, r) {
	c.save();

	c.translate(x, y);
	c.rotate(r * Math.PI / 180);

	c.strokeStyle = col;
	c.lineWidth = wid;

	c.strokeRect(-w / 2, -h / 2, w, h);

	c.restore();
}