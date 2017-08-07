//本文件由程序自动生成,请不要自行修改
var fastCSharpAPI;
(function (fastCSharpAPI) {
    var ajax;
    (function (ajax) {
        var pub = (function () {
            function pub() {
            }
            pub.OpenFile = function (file, Callback) {
                if (Callback === void 0) { Callback = null; }
                fastCSharp.Pub.GetAjaxPost()('pub.OpenFile', { file: file }, Callback);
            };
            return pub;
        }());
        ajax.pub = pub;
    })(ajax = fastCSharpAPI.ajax || (fastCSharpAPI.ajax = {}));
})(fastCSharpAPI || (fastCSharpAPI = {}));
var fastCSharpAPI;
(function (fastCSharpAPI) {
    var include = (function () {
        function include() {
        }
        include.codeMenu = function (file, Callback) {
            if (Callback === void 0) { Callback = null; }
            fastCSharp.Pub.GetAjaxGet()('/include/codeMenu.html', { file: file }, Callback);
        };
        return include;
    }());
    fastCSharpAPI.include = include;
})(fastCSharpAPI || (fastCSharpAPI = {}));
