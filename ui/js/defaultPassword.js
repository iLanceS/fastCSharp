/// <reference path = "./base.page.ts" />
'use strict';
//默认密码输入框	<input defaultpassword="{PasswordId:'YYY'}" id="XXX" type="text" /><input id="YYY" type="text" />
var fastCSharp;
(function (fastCSharp) {
    var DefaultPassword = (function () {
        function DefaultPassword(Parameter) {
            fastCSharp.Pub.GetParameter(this, DefaultPassword.DefaultParameter, Parameter);
            this.Start(this.Event || fastCSharp.DeclareEvent.Default);
        }
        DefaultPassword.prototype.Start = function (Event) {
            if (!Event.IsGetOnly) {
                var Element = fastCSharp.HtmlElement.$Id(this.Id), Input = Element.Element0(), Password = fastCSharp.HtmlElement.$Id(this.PasswordId);
                if (Input != this.Element) {
                    this.Element = Input;
                    Password.AddEvent('blur', fastCSharp.Pub.ThisFunction(this, this.OnBlur));
                }
                Element.Display(0);
                Password.Display(1).Focus0();
            }
        };
        DefaultPassword.prototype.OnBlur = function () {
            var Password = fastCSharp.HtmlElement.$Id(this.PasswordId);
            if (!Password.Element0().value) {
                Password.Display(0);
                fastCSharp.HtmlElement.$Id(this.Id).Display(1);
            }
        };
        DefaultPassword.DefaultParameter = { Id: null, Event: null, PasswordId: null };
        return DefaultPassword;
    }());
    fastCSharp.DefaultPassword = DefaultPassword;
    new fastCSharp.Declare(DefaultPassword, 'DefaultPassword', 'focus', 'Src');
})(fastCSharp || (fastCSharp = {}));
