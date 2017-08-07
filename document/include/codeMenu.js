/// <reference path = "../../ui/js/mouseMenu.ts" />
/// <reference path = "../viewJs/api.ts" />
/*include:js\mouseMenu*/
var CodeMenu = (function () {
    function CodeMenu() {
    }
    CodeMenu.Show = function (Menu) {
        Menu.MenuId = 'codeMenu';
        Menu.Type = Menu.Type || 'Top';
        if (CodeMenu.CurrentMenu != Menu)
            CodeMenu.Check(Menu);
    };
    CodeMenu.Check = function (Menu) {
        var SkinView = (this.CurrentMenu = Menu).SkinView, Item = this.Items[SkinView.File];
        Menu.CheckMenuParameter();
        if (Item) {
            SkinView.Item = Item;
            Menu.IsShow = Item != this.NullItem;
        }
        else {
            Menu.IsShow = false;
            fastCSharpAPI.include.codeMenu(SkinView.File, fastCSharp.Pub.ThisFunction(this, this.OnGetMenu, [Menu]));
        }
        fastCSharp.HtmlElement.$Id('codeMenu').Display(Menu.IsShow);
    };
    CodeMenu.OnGetMenu = function (Value, Menu) {
        if (Value && !Value.ErrorRequest) {
            this.Items[Menu.SkinView.File] = Value.Item || this.NullItem;
            if (Value.Item && Menu == this.CurrentMenu && Menu.IsOver) {
                Menu.SkinView.Item = Value.Item;
                Menu.IsShow = true;
                fastCSharp.Skin.Skins['codeMenu'].Show(Menu.SkinView);
                if (Menu.Type != 'Mouse')
                    Menu['To' + Menu.Type](null, fastCSharp.HtmlElement.$Id(Menu.Id));
            }
        }
    };
    CodeMenu.OpenFile = function () {
        fastCSharpAPI.ajax.pub.OpenFile(this.CurrentMenu.SkinView.Item.File.FullName);
        this.CurrentMenu.Hide();
    };
    CodeMenu.Items = {};
    CodeMenu.NullItem = {};
    return CodeMenu;
}());
