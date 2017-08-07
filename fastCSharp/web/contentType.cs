using System;
using System.Runtime.CompilerServices;

namespace fastCSharp.web
{
    /// <summary>
    /// 下载文件类型属性
    /// </summary>
    public sealed class contentTypeInfo : Attribute
    {
        /// <summary>
        /// 扩展名关联下载文件类型
        /// </summary>
        private static readonly stateSearcher.ascii<byte[]> contentTypes;
        /// <summary>
        /// 未知扩展名关联下载文件类型
        /// </summary>
        private static readonly byte[] unknownContentType;
        /// <summary>
        /// 文件扩展名
        /// </summary>
        public string ExtensionName;
        /// <summary>
        /// 下载文件类型名称
        /// </summary>
        public string Name;

        ///// <summary>
        ///// 获取扩展名关联下载文件类型
        ///// </summary>
        ///// <param name="start"></param>
        ///// <param name="end"></param>
        ///// <returns>扩展名关联下载文件类型</returns>
        //[MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        //internal unsafe static byte[] GetContentType(byte* start, byte* end)
        //{
        //    return contentTypes.Get(start, end, unknownContentType);
        //}
        /// <summary>
        /// 获取扩展名关联下载文件类型
        /// </summary>
        /// <param name="extensionName">扩展名</param>
        /// <returns>扩展名关联下载文件类型</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static byte[] GetContentType(ref subString extensionName)
        {
            return contentTypes.Get(ref extensionName, unknownContentType);
        }
        /// <summary>
        /// 获取扩展名关联下载文件类型
        /// </summary>
        /// <param name="extensionName">扩展名</param>
        /// <returns>扩展名关联下载文件类型</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static byte[] GetContentType(string extensionName)
        {
            return contentTypes.Get(extensionName, unknownContentType);
        }
        /// <summary>
        /// 获取扩展名关联下载文件类型
        /// </summary>
        /// <param name="extensionName">扩展名</param>
        /// <param name="defalutType">默认下载文件类型</param>
        /// <returns>扩展名关联下载文件类型</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static byte[] GetContentType(string extensionName, byte[] defalutType)
        {
            return contentTypes.Get(extensionName, defalutType);
        }
        static contentTypeInfo()
        {
            contentTypeInfo[] types = System.Enum.GetValues(typeof(contentType))
                .toArray<contentType>().getArray(value => fastCSharp.Enum<contentType, contentTypeInfo>.Array((int)value));
            contentTypes = new stateSearcher.ascii<byte[]>(types.getArray(value => value.ExtensionName), types.getArray(value => value.Name.getBytes()), true);
            unknownContentType = contentTypes.Get("*");
        }
    }
    /// <summary>
    /// 下载文件类型
    /// </summary>
#pragma warning disable
    public enum contentType
    {
        [contentTypeInfo(ExtensionName = "*", Name = "application/octet-stream")]
        _,
        [contentTypeInfo(ExtensionName = "323", Name = "text/h323")]
        _323,
        [contentTypeInfo(ExtensionName = "907", Name = "drawing/907")]
        _907,
        [contentTypeInfo(ExtensionName = "acp", Name = "audio/x-mei-aac")]
        acp,
        [contentTypeInfo(ExtensionName = "ai", Name = "application/postscript")]
        ai,
        [contentTypeInfo(ExtensionName = "aif", Name = "audio/aiff")]
        aif,
        [contentTypeInfo(ExtensionName = "aifc", Name = "audio/aiff")]
        aifc,
        [contentTypeInfo(ExtensionName = "aiff", Name = "audio/aiff")]
        aiff,
        [contentTypeInfo(ExtensionName = "apk", Name = "application/vnd.android.package-archive")]
        apk,
        [contentTypeInfo(ExtensionName = "asa", Name = "text/asa")]
        asa,
        [contentTypeInfo(ExtensionName = "asf", Name = "video/x-ms-asf")]
        asf,
        [contentTypeInfo(ExtensionName = "asp", Name = "text/asp")]
        asp,
        [contentTypeInfo(ExtensionName = "asx", Name = "video/x-ms-asf")]
        asx,
        [contentTypeInfo(ExtensionName = "au", Name = "audio/basic")]
        au,
        [contentTypeInfo(ExtensionName = "avi", Name = "video/avi")]
        avi,
        [contentTypeInfo(ExtensionName = "awf", Name = "application/vnd.adobe.workflow")]
        awf,
        [contentTypeInfo(ExtensionName = "biz", Name = "text/xml")]
        biz,
        [contentTypeInfo(ExtensionName = "bmp", Name = "image/msbitmap")]
        bmp,
        [contentTypeInfo(ExtensionName = "cat", Name = "application/vnd.ms-pki.seccat")]
        cat,
        [contentTypeInfo(ExtensionName = "cdf", Name = "application/x-netcdf")]
        cdf,
        [contentTypeInfo(ExtensionName = "cer", Name = "application/x-x509-ca-cert")]
        cer,
        [contentTypeInfo(ExtensionName = "class", Name = "java/*")]
        _class,
        [contentTypeInfo(ExtensionName = "cml", Name = "text/xml")]
        cml,
        [contentTypeInfo(ExtensionName = "crl", Name = "application/pkix-crl")]
        crl,
        [contentTypeInfo(ExtensionName = "crt", Name = "application/x-x509-ca-cert")]
        crt,
        [contentTypeInfo(ExtensionName = "css", Name = "text/css")]
        css,
        [contentTypeInfo(ExtensionName = "cur", Name = "image/x-icon")]
        cur,
        [contentTypeInfo(ExtensionName = "dcd", Name = "text/xml")]
        dcd,
        [contentTypeInfo(ExtensionName = "der", Name = "application/x-x509-ca-cert")]
        der,
        [contentTypeInfo(ExtensionName = "dll", Name = "application/x-msdownload")]
        dll,
        [contentTypeInfo(ExtensionName = "doc", Name = "application/msword")]
        doc,
        [contentTypeInfo(ExtensionName = "dot", Name = "application/msword")]
        dot,
        [contentTypeInfo(ExtensionName = "dtd", Name = "text/xml")]
        dtd,
        [contentTypeInfo(ExtensionName = "edn", Name = "application/vnd.adobe.edn")]
        edn,
        [contentTypeInfo(ExtensionName = "eml", Name = "message/rfc822")]
        eml,
        [contentTypeInfo(ExtensionName = "ent", Name = "text/xml")]
        ent,
        [contentTypeInfo(ExtensionName = "eot", Name = "application/font-eot")]
        eot,
        [contentTypeInfo(ExtensionName = "eps", Name = "application/postscript")]
        eps,
        [contentTypeInfo(ExtensionName = "exe", Name = "application/x-msdownload")]
        exe,
        [contentTypeInfo(ExtensionName = "fax", Name = "image/fax")]
        fax,
        [contentTypeInfo(ExtensionName = "fdf", Name = "application/vnd.fdf")]
        fdf,
        [contentTypeInfo(ExtensionName = "fif", Name = "application/fractals")]
        fif,
        [contentTypeInfo(ExtensionName = "flv", Name = "video/x-flv")]
        flv,
        [contentTypeInfo(ExtensionName = "fo", Name = "text/xml")]
        fo,
        [contentTypeInfo(ExtensionName = "gif", Name = "image/gif")]
        gif,
        [contentTypeInfo(ExtensionName = "hpg", Name = "application/x-hpgl")]
        hpg,
        [contentTypeInfo(ExtensionName = "hqx", Name = "application/mac-binhex40")]
        hqx,
        [contentTypeInfo(ExtensionName = "hta", Name = "application/hta")]
        hta,
        [contentTypeInfo(ExtensionName = "htc", Name = "text/x-component")]
        htc,
        [contentTypeInfo(ExtensionName = "htm", Name = "text/html")]
        htm,
        [contentTypeInfo(ExtensionName = "html", Name = "text/html")]
        html,
        [contentTypeInfo(ExtensionName = "htt", Name = "text/webviewhtml")]
        htt,
        [contentTypeInfo(ExtensionName = "htx", Name = "text/html")]
        htx,
        [contentTypeInfo(ExtensionName = "ico", Name = "image/x-icon")]
        ico,
        [contentTypeInfo(ExtensionName = "iii", Name = "application/x-iphone")]
        iii,
        [contentTypeInfo(ExtensionName = "img", Name = "application/x-img")]
        img,
        [contentTypeInfo(ExtensionName = "ins", Name = "application/x-internet-signup")]
        ins,
        [contentTypeInfo(ExtensionName = "isp", Name = "application/x-internet-signup")]
        isp,
        [contentTypeInfo(ExtensionName = "java", Name = "java/*")]
        java,
        [contentTypeInfo(ExtensionName = "jfif", Name = "image/jpeg")]
        jfif,
        [contentTypeInfo(ExtensionName = "jpe", Name = "image/jpeg")]
        jpe,
        [contentTypeInfo(ExtensionName = "jpeg", Name = "image/jpeg")]
        jpeg,
        [contentTypeInfo(ExtensionName = "jpg", Name = "image/jpeg")]
        jpg,
        [contentTypeInfo(ExtensionName = "js", Name = "application/x-javascript")]
        js,
        [contentTypeInfo(ExtensionName = "jsp", Name = "text/html")]
        jsp,
        [contentTypeInfo(ExtensionName = "la1", Name = "audio/x-liquid-file")]
        la1,
        [contentTypeInfo(ExtensionName = "lar", Name = "application/x-laplayer-reg")]
        lar,
        [contentTypeInfo(ExtensionName = "latex", Name = "application/x-latex")]
        latex,
        [contentTypeInfo(ExtensionName = "lavs", Name = "audio/x-liquid-secure")]
        lavs,
        [contentTypeInfo(ExtensionName = "lmsff", Name = "audio/x-la-lms")]
        lmsff,
        [contentTypeInfo(ExtensionName = "ls", Name = "application/x-javascript")]
        ls,
        [contentTypeInfo(ExtensionName = "m1v", Name = "video/x-mpeg")]
        m1v,
        [contentTypeInfo(ExtensionName = "m2v", Name = "video/x-mpeg")]
        m2v,
        [contentTypeInfo(ExtensionName = "m3u", Name = "audio/mpegurl")]
        m3u,
        [contentTypeInfo(ExtensionName = "m4e", Name = "video/mpeg4")]
        m4e,
        [contentTypeInfo(ExtensionName = "man", Name = "application/x-troff-man")]
        man,
        [contentTypeInfo(ExtensionName = "math", Name = "text/xml")]
        math,
        [contentTypeInfo(ExtensionName = "mdb", Name = "application/msaccess")]
        mdb,
        [contentTypeInfo(ExtensionName = "mfp", Name = "application/x-shockwave-flash")]
        mfp,
        [contentTypeInfo(ExtensionName = "mht", Name = "message/rfc822")]
        mht,
        [contentTypeInfo(ExtensionName = "mhtml", Name = "message/rfc822")]
        mhtml,
        [contentTypeInfo(ExtensionName = "mid", Name = "audio/mid")]
        mid,
        [contentTypeInfo(ExtensionName = "midi", Name = "audio/mid")]
        midi,
        [contentTypeInfo(ExtensionName = "mml", Name = "text/xml")]
        mml,
        [contentTypeInfo(ExtensionName = "mnd", Name = "audio/x-musicnet-download")]
        mnd,
        [contentTypeInfo(ExtensionName = "mns", Name = "audio/x-musicnet-stream")]
        mns,
        [contentTypeInfo(ExtensionName = "mocha", Name = "application/x-javascript")]
        mocha,
        [contentTypeInfo(ExtensionName = "movie", Name = "video/x-sgi-movie")]
        movie,
        [contentTypeInfo(ExtensionName = "mp1", Name = "audio/mp1")]
        mp1,
        [contentTypeInfo(ExtensionName = "mp2", Name = "audio/mp2")]
        mp2,
        [contentTypeInfo(ExtensionName = "mp2v", Name = "video/mpeg")]
        mp2v,
        [contentTypeInfo(ExtensionName = "mp3", Name = "audio/mp3")]
        mp3,
        [contentTypeInfo(ExtensionName = "mp4", Name = "video/mpeg4")]
        mp4,
        [contentTypeInfo(ExtensionName = "mpa", Name = "video/x-mpg")]
        mpa,
        [contentTypeInfo(ExtensionName = "mpd", Name = "application/vnd.ms-project")]
        mpd,
        [contentTypeInfo(ExtensionName = "mpe", Name = "video/x-mpeg")]
        mpe,
        [contentTypeInfo(ExtensionName = "mpeg", Name = "video/mpg")]
        mpeg,
        [contentTypeInfo(ExtensionName = "mpg", Name = "video/mpg")]
        mpg,
        [contentTypeInfo(ExtensionName = "mpga", Name = "audio/rn-mpeg")]
        mpga,
        [contentTypeInfo(ExtensionName = "mpp", Name = "application/vnd.ms-project")]
        mpp,
        [contentTypeInfo(ExtensionName = "mps", Name = "video/x-mpeg")]
        mps,
        [contentTypeInfo(ExtensionName = "mpt", Name = "application/vnd.ms-project")]
        mpt,
        [contentTypeInfo(ExtensionName = "mpv", Name = "video/mpg")]
        mpv,
        [contentTypeInfo(ExtensionName = "mpv2", Name = "video/mpeg")]
        mpv2,
        [contentTypeInfo(ExtensionName = "mpw", Name = "application/vnd.ms-project")]
        mpw,
        [contentTypeInfo(ExtensionName = "mpx", Name = "application/vnd.ms-project")]
        mpx,
        [contentTypeInfo(ExtensionName = "mtx", Name = "text/xml")]
        mtx,
        [contentTypeInfo(ExtensionName = "mxp", Name = "application/x-mmxp")]
        mxp,
        [contentTypeInfo(ExtensionName = "net", Name = "image/pnetvue")]
        net,
        [contentTypeInfo(ExtensionName = "nws", Name = "message/rfc822")]
        nws,
        [contentTypeInfo(ExtensionName = "odc", Name = "text/x-ms-odc")]
        odc,
        [contentTypeInfo(ExtensionName = "otf", Name = "application/font-otf")]
        otf,
        [contentTypeInfo(ExtensionName = "p10", Name = "application/pkcs10")]
        p10,
        [contentTypeInfo(ExtensionName = "p12", Name = "application/x-pkcs12")]
        p12,
        [contentTypeInfo(ExtensionName = "p7b", Name = "application/x-pkcs7-certificates")]
        p7b,
        [contentTypeInfo(ExtensionName = "p7c", Name = "application/pkcs7-mime")]
        p7c,
        [contentTypeInfo(ExtensionName = "p7m", Name = "application/pkcs7-mime")]
        p7m,
        [contentTypeInfo(ExtensionName = "p7r", Name = "application/x-pkcs7-certreqresp")]
        p7r,
        [contentTypeInfo(ExtensionName = "p7s", Name = "application/pkcs7-signature")]
        p7s,
        [contentTypeInfo(ExtensionName = "pcx", Name = "image/x-pcx")]
        pcx,
        [contentTypeInfo(ExtensionName = "pdf", Name = "application/pdf")]
        pdf,
        [contentTypeInfo(ExtensionName = "pdx", Name = "application/vnd.adobe.pdx")]
        pdx,
        [contentTypeInfo(ExtensionName = "pfx", Name = "application/x-pkcs12")]
        pfx,
        [contentTypeInfo(ExtensionName = "pic", Name = "application/x-pic")]
        pic,
        [contentTypeInfo(ExtensionName = "pko", Name = "application/vnd.ms-pki.pko")]
        pko,
        [contentTypeInfo(ExtensionName = "pl", Name = "application/x-perl")]
        pl,
        [contentTypeInfo(ExtensionName = "plg", Name = "text/html")]
        plg,
        [contentTypeInfo(ExtensionName = "pls", Name = "audio/scpls")]
        pls,
        [contentTypeInfo(ExtensionName = "png", Name = "image/png")]
        png,
        [contentTypeInfo(ExtensionName = "pot", Name = "application/vnd.ms-powerpoint")]
        pot,
        [contentTypeInfo(ExtensionName = "ppa", Name = "application/vnd.ms-powerpoint")]
        ppa,
        [contentTypeInfo(ExtensionName = "pps", Name = "application/vnd.ms-powerpoint")]
        pps,
        [contentTypeInfo(ExtensionName = "ppt", Name = "application/vnd.ms-powerpoint")]
        ppt,
        [contentTypeInfo(ExtensionName = "prf", Name = "application/pics-rules")]
        prf,
        [contentTypeInfo(ExtensionName = "ps", Name = "application/postscript")]
        ps,
        [contentTypeInfo(ExtensionName = "pwz", Name = "application/vnd.ms-powerpoint")]
        pwz,
        [contentTypeInfo(ExtensionName = "r3t", Name = "text/vnd.rn-realtext3d")]
        r3t,
        [contentTypeInfo(ExtensionName = "ra", Name = "audio/vnd.rn-realaudio")]
        ra,
        [contentTypeInfo(ExtensionName = "ram", Name = "audio/x-pn-realaudio")]
        ram,
        [contentTypeInfo(ExtensionName = "rat", Name = "application/rat-file")]
        rat,
        [contentTypeInfo(ExtensionName = "rdf", Name = "text/xml")]
        rdf,
        [contentTypeInfo(ExtensionName = "rec", Name = "application/vnd.rn-recording")]
        rec,
        [contentTypeInfo(ExtensionName = "rjs", Name = "application/vnd.rn-realsystem-rjs")]
        rjs,
        [contentTypeInfo(ExtensionName = "rjt", Name = "application/vnd.rn-realsystem-rjt")]
        rjt,
        [contentTypeInfo(ExtensionName = "rm", Name = "application/vnd.rn-realmedia")]
        rm,
        [contentTypeInfo(ExtensionName = "rmf", Name = "application/vnd.adobe.rmf")]
        rmf,
        [contentTypeInfo(ExtensionName = "rmi", Name = "audio/mid")]
        rmi,
        [contentTypeInfo(ExtensionName = "rmj", Name = "application/vnd.rn-realsystem-rmj")]
        rmj,
        [contentTypeInfo(ExtensionName = "rmm", Name = "audio/x-pn-realaudio")]
        rmm,
        [contentTypeInfo(ExtensionName = "rmp", Name = "application/vnd.rn-rn_music_package")]
        rmp,
        [contentTypeInfo(ExtensionName = "rms", Name = "application/vnd.rn-realmedia-secure")]
        rms,
        [contentTypeInfo(ExtensionName = "rmvb", Name = "application/vnd.rn-realmedia-vbr")]
        rmvb,
        [contentTypeInfo(ExtensionName = "rmx", Name = "application/vnd.rn-realsystem-rmx")]
        rmx,
        [contentTypeInfo(ExtensionName = "rnx", Name = "application/vnd.rn-realplayer")]
        rnx,
        [contentTypeInfo(ExtensionName = "rp", Name = "image/vnd.rn-realpix")]
        rp,
        [contentTypeInfo(ExtensionName = "rpm", Name = "audio/x-pn-realaudio-plugin")]
        rpm,
        [contentTypeInfo(ExtensionName = "rsml", Name = "application/vnd.rn-rsml")]
        rsml,
        [contentTypeInfo(ExtensionName = "rt", Name = "text/vnd.rn-realtext")]
        rt,
        [contentTypeInfo(ExtensionName = "rtf", Name = "application/msword")]
        rtf,
        [contentTypeInfo(ExtensionName = "rv", Name = "video/vnd.rn-realvideo")]
        rv,
        [contentTypeInfo(ExtensionName = "sit", Name = "application/x-stuffit")]
        sit,
        [contentTypeInfo(ExtensionName = "smi", Name = "application/smil")]
        smi,
        [contentTypeInfo(ExtensionName = "smil", Name = "application/smil")]
        smil,
        [contentTypeInfo(ExtensionName = "snd", Name = "audio/basic")]
        snd,
        [contentTypeInfo(ExtensionName = "sol", Name = "text/plain")]
        sol,
        [contentTypeInfo(ExtensionName = "sor", Name = "text/plain")]
        sor,
        [contentTypeInfo(ExtensionName = "spc", Name = "application/x-pkcs7-certificates")]
        spc,
        [contentTypeInfo(ExtensionName = "spl", Name = "application/futuresplash")]
        spl,
        [contentTypeInfo(ExtensionName = "spp", Name = "text/xml")]
        spp,
        [contentTypeInfo(ExtensionName = "ssm", Name = "application/streamingmedia")]
        ssm,
        [contentTypeInfo(ExtensionName = "sst", Name = "application/vnd.ms-pki.certstore")]
        sst,
        [contentTypeInfo(ExtensionName = "stl", Name = "application/vnd.ms-pki.stl")]
        stl,
        [contentTypeInfo(ExtensionName = "stm", Name = "text/html")]
        stm,
        [contentTypeInfo(ExtensionName = "svg", Name = "text/xml")]
        svg,
        [contentTypeInfo(ExtensionName = "swf", Name = "application/x-shockwave-flash")]
        swf,
        [contentTypeInfo(ExtensionName = "tif", Name = "image/tiff")]
        tif,
        [contentTypeInfo(ExtensionName = "tiff", Name = "image/tiff")]
        tiff,
        [contentTypeInfo(ExtensionName = "tld", Name = "text/xml")]
        tld,
        [contentTypeInfo(ExtensionName = "torrent", Name = "application/x-bittorrent")]
        torrent,
        [contentTypeInfo(ExtensionName = "tsd", Name = "text/xml")]
        tsd,
        [contentTypeInfo(ExtensionName = "txt", Name = "text/plain")]
        txt,
        [contentTypeInfo(ExtensionName = "uin", Name = "application/x-icq")]
        uin,
        [contentTypeInfo(ExtensionName = "uls", Name = "text/iuls")]
        uls,
        [contentTypeInfo(ExtensionName = "vcf", Name = "text/x-vcard")]
        vcf,
        [contentTypeInfo(ExtensionName = "vdx", Name = "application/vnd.visio")]
        vdx,
        [contentTypeInfo(ExtensionName = "vml", Name = "text/xml")]
        vml,
        [contentTypeInfo(ExtensionName = "vpg", Name = "application/x-vpeg005")]
        vpg,
        [contentTypeInfo(ExtensionName = "vsd", Name = "application/vnd.visio")]
        vsd,
        [contentTypeInfo(ExtensionName = "vss", Name = "application/vnd.visio")]
        vss,
        [contentTypeInfo(ExtensionName = "vst", Name = "application/vnd.visio")]
        vst,
        [contentTypeInfo(ExtensionName = "vsw", Name = "application/vnd.visio")]
        vsw,
        [contentTypeInfo(ExtensionName = "vsx", Name = "application/vnd.visio")]
        vsx,
        [contentTypeInfo(ExtensionName = "vtx", Name = "application/vnd.visio")]
        vtx,
        [contentTypeInfo(ExtensionName = "vxml", Name = "text/xml")]
        vxml,
        [contentTypeInfo(ExtensionName = "wav", Name = "audio/wav")]
        wav,
        [contentTypeInfo(ExtensionName = "wax", Name = "audio/x-ms-wax")]
        wax,
        [contentTypeInfo(ExtensionName = "wbmp", Name = "image/vnd.wap.wbmp")]
        wbmp,
        [contentTypeInfo(ExtensionName = "wiz", Name = "application/msword")]
        wiz,
        [contentTypeInfo(ExtensionName = "wm", Name = "video/x-ms-wm")]
        wm,
        [contentTypeInfo(ExtensionName = "wma", Name = "audio/x-ms-wma")]
        wma,
        [contentTypeInfo(ExtensionName = "wmd", Name = "application/x-ms-wmd")]
        wmd,
        [contentTypeInfo(ExtensionName = "wml", Name = "text/vnd.wap.wml")]
        wml,
        [contentTypeInfo(ExtensionName = "wmv", Name = "video/x-ms-wmv")]
        wmv,
        [contentTypeInfo(ExtensionName = "wmx", Name = "video/x-ms-wmx")]
        wmx,
        [contentTypeInfo(ExtensionName = "wmz", Name = "application/x-ms-wmz")]
        wmz,
        [contentTypeInfo(ExtensionName = "woff", Name = "application/font-woff")]
        woff,
        [contentTypeInfo(ExtensionName = "wpl", Name = "application/vnd.ms-wpl")]
        wpl,
        [contentTypeInfo(ExtensionName = "wsc", Name = "text/scriptlet")]
        wsc,
        [contentTypeInfo(ExtensionName = "wsdl", Name = "text/xml")]
        wsdl,
        [contentTypeInfo(ExtensionName = "wvx", Name = "video/x-ms-wvx")]
        wvx,
        [contentTypeInfo(ExtensionName = "xdp", Name = "application/vnd.adobe.xdp")]
        xdp,
        [contentTypeInfo(ExtensionName = "xdr", Name = "text/xml")]
        xdr,
        [contentTypeInfo(ExtensionName = "xfd", Name = "application/vnd.adobe.xfd")]
        xfd,
        [contentTypeInfo(ExtensionName = "xfdf", Name = "application/vnd.adobe.xfdf")]
        xfdf,
        [contentTypeInfo(ExtensionName = "xhtml", Name = "text/html")]
        xhtml,
        [contentTypeInfo(ExtensionName = "xls", Name = "application/vnd.ms-excel")]
        xls,
        [contentTypeInfo(ExtensionName = "xml", Name = "text/xml")]
        xml,
        [contentTypeInfo(ExtensionName = "xpl", Name = "audio/scpls")]
        xpl,
        [contentTypeInfo(ExtensionName = "xq", Name = "text/xml")]
        xq,
        [contentTypeInfo(ExtensionName = "xql", Name = "text/xml")]
        xql,
        [contentTypeInfo(ExtensionName = "xquery", Name = "text/xml")]
        xquery,
        [contentTypeInfo(ExtensionName = "xsd", Name = "text/xml")]
        xsd,
        [contentTypeInfo(ExtensionName = "xsl", Name = "text/xml")]
        xsl,
        [contentTypeInfo(ExtensionName = "xslt", Name = "text/xml")]
        xslt,
    }
#pragma warning restore
}
