/// <reference path = "../../../ui/js/base.page.ts" />
/// <reference path = "./webPath.ts" />
'use strict';
module Demo {
    export class Student {
        private Id: number;
        private Path: fastCSharpPath.Student;
        constructor(Value: Object) {
            fastCSharp.Pub.Copy(this, Value);
            this.Path = new fastCSharpPath.Student(this.Id);
        }
    }
}
fastCSharp.Pub.LoadViewType(Demo.Student);