/// <reference path = "../../../ui/js/base.page.ts" />
/// <reference path = "./webPath.ts" />
'use strict';
module Demo {
    export class Class {
        private Id: number;
        private Path: fastCSharpPath.Class;
        constructor(Value: Object) {
            fastCSharp.Pub.Copy(this, Value);
            this.Path = new fastCSharpPath.Class(this.Id);
        }
    }
}
fastCSharp.Pub.LoadViewType(Demo.Class);