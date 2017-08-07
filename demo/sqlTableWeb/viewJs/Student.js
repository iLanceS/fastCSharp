/// <reference path = "../../../ui/js/base.page.ts" />
/// <reference path = "./webPath.ts" />
'use strict';
var Demo;
(function (Demo) {
    var Student = (function () {
        function Student(Value) {
            fastCSharp.Pub.Copy(this, Value);
            this.Path = new fastCSharpPath.Student(this.Id);
        }
        return Student;
    }());
    Demo.Student = Student;
})(Demo || (Demo = {}));
fastCSharp.Pub.LoadViewType(Demo.Student);
