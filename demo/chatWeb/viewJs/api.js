//本文件由程序自动生成,请不要自行修改
var fastCSharpAPI;
(function (fastCSharpAPI) {
    var ajax;
    (function (ajax) {
        var user = (function () {
            function user() {
            }
            user.Login = function (user, version, Callback) {
                if (Callback === void 0) { Callback = null; }
                fastCSharp.Pub.GetAjaxPost()('user.Login', { user: user, version: version }, Callback);
            };
            user.Logout = function (user, Callback) {
                if (Callback === void 0) { Callback = null; }
                fastCSharp.Pub.GetAjaxPost()('user.Logout', { user: user }, Callback);
            };
            user.Send = function (user, message, users, Callback) {
                if (Callback === void 0) { Callback = null; }
                fastCSharp.Pub.GetAjaxPost()('user.Send', { user: user, message: message, users: users }, Callback);
            };
            return user;
        }());
        ajax.user = user;
    })(ajax = fastCSharpAPI.ajax || (fastCSharpAPI.ajax = {}));
})(fastCSharpAPI || (fastCSharpAPI = {}));
