
Function.prototype.method = function (name, func) {

    if (!this.prototype[name]) {
        this.prototype[name] = func;
    }
    return this;
}


String.method('trim', function () {
    return this.replace(/^s+|\s+$/g, '');
});

var myObject = function () {
    var value = 0;
    return {
        increment: function (inc) {
            value += typeof inc === 'number' ? inc : 1;
        },
        getValue: function () {
            return value;
        }
    }
}();
