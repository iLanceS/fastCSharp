//本文件由程序自动生成,请不要自行修改
module fastCSharpAPI.ajax {
	export class pub {
		
		static OpenFile(file,Callback = null) {
			fastCSharp.Pub.GetAjaxPost()('pub.OpenFile',{file: file }, Callback);	
		}
		
	}
}
module fastCSharpAPI {
	export class include {		static codeMenu(file,Callback = null) {
			fastCSharp.Pub.GetAjaxGet()('/include/codeMenu.html',{file: file }, Callback);	
		}
	}
}
