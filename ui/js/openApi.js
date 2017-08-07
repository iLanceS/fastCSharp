var fastCSharp;
(function (fastCSharp) {
    var OpenApi = (function () {
        function OpenApi(StateAjaxCallName) {
            if (StateAjaxCallName === void 0) { StateAjaxCallName = 'pub.GetOpenApiState'; }
            this.StateAjaxCallName = StateAjaxCallName;
            this.APIs = {};
        }
        OpenApi.prototype.Login = function (Name) {
            var Api = this.APIs[Name];
            if (Api && Api.Url) {
                if (Api.IsState == null || Api.IsState)
                    fastCSharp.HttpRequest.Post(this.StateAjaxCallName, null, fastCSharp.Pub.ThisFunction(this, this.OnState, [Api]));
                else
                    location = Api.Url;
            }
            else
                fastCSharp.Pub.Alert('未找到第三方 ' + Name + ' API信息');
        };
        OpenApi.prototype.OnState = function (Value, Api) {
            if (Value = Value.__AJAXRETURN__) {
                if (this.IsLoginCookie == null || this.IsLoginCookie)
                    fastCSharp.Cookie.Default.Write({ Name: 'OpenLoginUrl', Value: location.toString() });
                location = Api.Url + '&state=' + Value;
            }
            else
                fastCSharp.HttpRequest.CheckError(Value);
        };
        OpenApi.prototype.Location = function (Url) {
            var Value = fastCSharp.Cookie.Default.Read('OpenLoginUrl');
            if (Value)
                fastCSharp.Cookie.Default.Write({ Name: 'OpenLoginUrl' });
            location.replace(Value || Url);
        };
        OpenApi.Default = new OpenApi();
        return OpenApi;
    }());
    fastCSharp.OpenApi = OpenApi;
})(fastCSharp || (fastCSharp = {}));
