const assert = require('node:assert/strict');
const fs = require('node:fs');
const vm = require('node:vm');
const values = new Map();
const context = {
    document: {},
    $: key => ({
        ready() {},
        val(value) { if (arguments.length) values.set(key, value); return values.get(key); },
        attr() {}, text() {}, find() { return { remove() {} }; },
        DataTable() { return { destroy() {} }; }
    })
};
vm.createContext(context);
vm.runInContext(fs.readFileSync('CVSalis/wwwroot/myjs/CVDetails.js', 'utf8'), context);
assert.equal(context.imageUrl('new.png'), '/Image/new.png');
assert.equal(context.imageUrl('/Image/new.png'), '/Image/new.png');
assert.equal(context.imageUrl(null), '');
context.pathImage = '/Image/previous-person.png';
context.table = { clear() {} };
context.OnClearForm();
assert.equal(context.pathImage, '');
assert.equal(values.get('#upload'), '');
values.set('#expSallary', '65000000');
values.set('#countExp', '6');
context.$.ajax = options => {
    assert.equal(options.data.total_exp, '6');
    assert.equal(options.data.expectation_sallary, '65000000');
    assert.equal(options.data.image, '/Image/new.png');
};
// Ignore display changes while checking the actual update payload.
const original = context.$;
context.$ = Object.assign(key => ({ ...original(key), css() {} }), original);
context.pathImage = context.imageUrl('new.png');
context.OnUpdateCV();
console.log('PASS: legacy/current photo URLs, reset, update photo and experience payload');
