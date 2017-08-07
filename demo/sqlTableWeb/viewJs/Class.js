/// <reference path = "../../../ui/js/base.page.ts" />
/// <reference path = "./webPath.ts" />
'use strict';
var Demo;
(function (Demo) {
    var Class = (function () {
        function Class(Value) {
            fastCSharp.Pub.Copy(this, Value);
            this.Path = new fastCSharpPath.Class(this.Id);
        }
        return Class;
    }());
    Demo.Class = Class;
})(Demo || (Demo = {}));
fastCSharp.Pub.LoadViewType(Demo.Class);
