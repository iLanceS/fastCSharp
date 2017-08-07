/// <reference path = "../../ui/js/mouseMenu.ts" />
/// <reference path = "../viewJs/api.ts" />
interface ICodeMenuView {
    File: string;
    Item: ICodeMenuItem;
}
interface ICodeMenuItem {
    File: ICodeMenuFileInfo;
}
interface ICodeMenuFileInfo {
    FullName: string;
}
/*include:js\mouseMenu*/
class CodeMenu {
    private static CurrentMenu: fastCSharp.MouseMenu;
    private static Items: { [key: string]: ICodeMenuItem } = {};
    private static NullItem: ICodeMenuItem = {} as ICodeMenuItem;
    private static Show(Menu: fastCSharp.MouseMenu) {
        Menu.MenuId = 'codeMenu';
        Menu.Type = Menu.Type || 'Top';
        if (CodeMenu.CurrentMenu != Menu) CodeMenu.Check(Menu);
    }
    private static Check(Menu: fastCSharp.MouseMenu) {
        var SkinView = (this.CurrentMenu = Menu).SkinView as ICodeMenuView, Item = this.Items[SkinView.File];
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
    }
    private static OnGetMenu(Value: ICodeMenuView, Menu: fastCSharp.MouseMenu) {
        if (Value && !(Value as Object as fastCSharp.IHttpRequestReturn).ErrorRequest) {
            this.Items[(Menu.SkinView as ICodeMenuView).File] = Value.Item || this.NullItem;
            if (Value.Item && Menu == this.CurrentMenu && Menu.IsOver) {
                (Menu.SkinView as ICodeMenuView).Item = Value.Item;
                Menu.IsShow = true;
                fastCSharp.Skin.Skins['codeMenu'].Show(Menu.SkinView);
                if (Menu.Type != 'Mouse') Menu['To' + Menu.Type](null, fastCSharp.HtmlElement.$Id(Menu.Id));
            }
        }
    }
    private static OpenFile() {
        fastCSharpAPI.ajax.pub.OpenFile((this.CurrentMenu.SkinView as ICodeMenuView).Item.File.FullName);
        this.CurrentMenu.Hide();
    }
}