var zoom=100;
var transX=-453;
var transY=-1348;
var aktiv,svgdok,xTrans,yTrans,zoompoint,zoomtext,svgmap,navigation,docroot,MapScrollTimeout,MapZoomTimeout,debug,debug2,mittelpunkt,mX,mY,maxX,maxY,minX,minY,grundstuecke,w,h,bittewarten,ttrelem,tttelem,tooltip,oldColor,str_hh,str_h,str_n,str_nn;
var xUrsprung=6102.94117647059;
var yUrsprung=10382.3529411765;
var scale=2.94117647058824;
var min_x=4500;
var min_y=6500;
var size_y=6500;
var minLoadedX=4500;
var minLoadedY=6500;
var maxLoadedX=10000;
var maxLoadedY=13000;
var hover=1;
window.translateMap=translateMap;
function initialize1(evt) {
svgdok=evt.getTarget().getOwnerDocument();
zoompoint=svgdok.getElementById("zoompoint");
zoomtext=svgdok.getElementById("zoomtext");
svgmap=svgdok.getElementById("whole_graph");
navigation=svgdok.getElementById("navigation");
grundstuecke=svgdok.getElementById("grundstuecke");
ttrelem=svgdok.getElementById("ttr");
tttelem=svgdok.getElementById("ttt");
tooltip=svgdok.getElementById("tooltip");

w=getInnerWidth();
h=getInnerHeight();
// debug2 = svgdok.getElementById("debug2");
docroot=document.rootElement;
docroot.addEventListener("SVGScroll",syncMenu,false);
docroot.addEventListener("SVGResize",syncMenu,false);
docroot.addEventListener("SVGZoom",syncMenu,false);
docroot.addEventListener("mouseup",translateMapEnd,false);
docroot.addEventListener("mouseup",zoomMapEnd,false);
mX=Math.round((w/2)*scale+xUrsprung);
mY=Math.round(yUrsprung-(h/2)*scale);
maxX=Math.round(xUrsprung+w*scale);
minY=Math.round(yUrsprung-h*scale);
minX=Math.round(xUrsprung);
maxY=Math.round(yUrsprung);
//bittewarten=svgdok.getElementById("bittewarten");
//alert("translate("+w+","+h+")");
//bittewarten.setAttribute("transform","translate("+(-(w-200)/2)+","+(-(h-70)/2)+")");
docroot.currentScale=docroot.currentScale;

var svgdoc = svgdok.getDocumentElement();
var ddbox = svgdoc.getAttribute("viewBox");

svgdoc.removeAttribute("viewBox");

svgdoc.setAttribute("height","100%");
svgdoc.setAttribute("width","100%");

var ss_box_array;
ss_box_array = ddbox.split(" ");
var s1 = w/ss_box_array[2];
var s2 = 0;
var s3 = 0;
var s4 = h/ss_box_array[3];
var s5 = -ss_box_array[0]*s1;
var s6 = -ss_box_array[1]*s4;

svgmap.setAttribute("transform","matrix("+s1+","+s2+","+s3+","+s4+","+s5+","+s6+")");


// var gesamt = 0;
// for(var i = 'A'.charCodeAt(0); i <= 'Z'.charCodeAt(0); i++) {
// laenge = texttest.getComputedTextLength() / 100;
// gesamt += laenge;
// texttest = svgdok.getElementById('text-A');
// texttest.firstChild.data = 'gemessen ' + (texttest.getComputedTextLength()) + ' gerechnet ' + gesamt;
}
function syncMenu(evt) {
var s = 1/docroot.currentScale;
var ct = docroot.currentTranslate;
var tf="matrix("+s+" 0 0 "+s+" "+(-ct.x)*s+" "+(-ct.y)*s+")";
navigation.setAttribute( "transform", tf );
var scaleS = scale * s;
var scaleSX = xUrsprung - scaleS * ct.x;
var scaleSY = yUrsprung + ct.y * scaleS;
mX = Math.round( (w/2) * scaleS + scaleSX );
mY = Math.round(scaleSY - (h/2) * scaleS);
maxX = Math.round(scaleSX + w * scaleS);
minY = Math.round(scaleSY - h * scaleS);
minX = Math.round(scaleSX);
maxY = Math.round(scaleSY);
// debug2.firstChild.data = ' ( ' + maxY + ' / ' + maxLoadedY + ' / ' + maxX + ' / ' + maxLoadedX + ' ) ';
if (minY < minLoadedY) { ladeSektoren(); } else if (minX < minLoadedX) { ladeSektoren(); } else if (maxY > maxLoadedY) { ladeSektoren(); } else if (maxX > maxLoadedX) { ladeSektoren(); }
}
function zoomMapStart(evt,zoom) {
var specialFactor = docroot.currentScale / 2 * (1 - zoom);
docroot.currentScale = docroot.currentScale * zoom;
docroot.currentTranslate.y = docroot.currentTranslate.y + (getInnerHeight() * specialFactor);
docroot.currentTranslate.x = docroot.currentTranslate.x + (getInnerWidth() * specialFactor);
if( MapZoomTimeout )
clearTimeout( MapZoomTimeout );
MapZoomTimeout = setTimeout( "zoomMap(" + zoom + ")", 1000/docroot.currentScale );
}
function zoomMap(zoom) {
var specialFactor = docroot.currentScale / 2 * (1 - zoom);
docroot.currentScale = docroot.currentScale * zoom;
docroot.currentTranslate.y = docroot.currentTranslate.y + (getInnerHeight() * specialFactor);
docroot.currentTranslate.x = docroot.currentTranslate.x + (getInnerWidth() * specialFactor);
if( MapZoomTimeout )
clearTimeout( MapZoomTimeout );
MapZoomTimeout = setTimeout( "zoomMap(" + zoom + ")", 1000/docroot.currentScale );
}
function zoomMapEnd(evt) {
if( MapZoomTimeout )
clearTimeout( MapZoomTimeout );
MapZoomTimeout = null;
}
function translateMapStart(evt,x,y) {
docroot.currentTranslate.y = docroot.currentTranslate.y-Math.round(y*docroot.currentScale);
docroot.currentTranslate.x = docroot.currentTranslate.x-Math.round(x*docroot.currentScale);
if( MapScrollTimeout )
clearTimeout( MapScrollTimeout );
MapScrollTimeout = setTimeout( "translateMap(" + x + "," + y + ")", 500 );
}
function translateMap(x,y) {
docroot.currentTranslate.y = docroot.currentTranslate.y-Math.round(y*docroot.currentScale);
docroot.currentTranslate.x = docroot.currentTranslate.x-Math.round(x*docroot.currentScale);
if( MapScrollTimeout )
clearTimeout( MapScrollTimeout );
MapScrollTimeout = setTimeout( "translateMap(" + x + "," + y + ")", 100 );
}
function translateMapEnd(evt) {
if( MapScrollTimeout )
clearTimeout( MapScrollTimeout );
MapScrollTimeout = null;
}
function changeBVG(evt) {
var bvgObj = svgdok.getElementById("bvg");
var bvgCheckObj = svgdok.getElementById("bvgcheck");
if ( bvgObj.getAttribute("visibility") == "visible" ) {
bvgObj.setAttribute("visibility","hidden");
bvgCheckObj.setAttribute("visibility","hidden");
} else {
bvgObj.setAttribute("visibility","visible");
bvgCheckObj.setAttribute("visibility","visible");
}
}
function changeHover(evt) {
if (hover==1) {
svgdok.getElementById("hovercheck").setAttribute("visibility","hidden");
hover=0;
} else {
svgdok.getElementById("hovercheck").setAttribute("visibility","visible");
hover=1;
}
}
function ladeSektoren(evt) {
bittewarten.setAttribute("visibility","visible");
getURL("svgmap.cgi?action=getsectors&minX=" + minX + "&minY=" + minY + "&maxX=" + maxX + "&maxY=" + maxY + "&minLoadedX=" + minLoadedX + "&minLoadedY=" + minLoadedY + "&maxLoadedX=" + maxLoadedX + "&maxLoadedY=" + maxLoadedY + "&scale=" + scale + "&min_x=" + min_x + "&min_y=" + min_y + "&size_y=" + size_y,addSektoren);
}
function addSektoren(urlRequestStatus) {
var ausgabe;
var n = svgdok.getElementById("N");
var nn = svgdok.getElementById("NN");
var h = svgdok.getElementById("H");
var hh = svgdok.getElementById("HH");
var nBorder = svgdok.getElementById("NBorder");
var nnBorder = svgdok.getElementById("NNBorder");
var hBorder = svgdok.getElementById("HBorder");
var hhBorder = svgdok.getElementById("HHBorder");

if(urlRequestStatus.success) {
ausgabe=urlRequestStatus.content.split(";");
minMax = ausgabe[0].split(" ");
minLoadedX = minMax[0];
minLoadedY = minMax[1];
maxLoadedX = minMax[2];
maxLoadedY = minMax[3];

var laenge = ausgabe.length;
for(var i = 1; i < laenge; i++) {
strasse = ausgabe[i].split("|");
newPath = svgdok.createElement("path");
newPath.setAttribute("d","M"+strasse[1]);
newPath2 = svgdok.createElement("path");
newPath2.setAttribute("d","M"+strasse[1]);
switch(strasse[0]) {
case "N": nBorder.appendChild(newPath); break;
case "NN": nnBorder.appendChild(newPath); break;
case "H": hBorder.appendChild(newPath); break;
case "HH": hhBorder.appendChild(newPath); break;
}
switch(strasse[0]) {
case "N": n.appendChild(newPath2); break;
case "NN": nn.appendChild(newPath2); break;
case "H": h.appendChild(newPath2); break;
case "HH": hh.appendChild(newPath2); break;
}
}
}
bittewarten.setAttribute("visibility","hidden");
}
function showGrundstuecke(evt) {
bittewarten.setAttribute("visibility","visible");
getURL("svgmap.cgi?action=getareas&x=" + mX + "&y=" + mY + "&scale=" + scale + "&min_x=" + min_x + "&min_y=" + min_y + "&size_y=" + size_y,callback);
}
function callback(urlRequestStatus) {
var ausgabe;

if(urlRequestStatus.success) {
ausgabe=urlRequestStatus.content.split("|");

var laenge = ausgabe.length;
for(var i = 0; i < laenge; i++) {
newPath = svgdok.createElement("path");
newPath.setAttribute("d",ausgabe[i]);
grundstuecke.appendChild(newPath);
}
}
bittewarten.setAttribute("visibility","hidden");
}
function ShowTooltip(evt,text) {
if (hover) {
var strz=evt.getTarget();
var pos=strz.getAttribute("id").indexOf("_");
if (pos==-1) {
strz.parentNode.setAttribute("stroke","black");
strz.parentNode.setAttribute("stroke-width",eval(strz.parentNode.parentNode.getStyle().getPropertyValue("stroke-width")) + 2);
} else {
var strID=strz.getAttribute("id").substr(0,pos+1);

}
tttelem.childNodes.item(0).data=text;
ttrelem.setAttribute("x",evt.clientX+25);
ttrelem.setAttribute("y",evt.clientY+25);
tttelem.setAttribute("x",evt.clientX+30);
tttelem.setAttribute("y",evt.clientY+37);
ttrelem.setAttribute("width",tttelem.getComputedTextLength()+10);
tooltip.setAttribute("visibility","visible");
}
}
function HideTooltip(evt) {
if (hover) {
var strz=evt.getTarget();
var pos=strz.getAttribute("id").indexOf("_");
if (pos==-1) {
strz.parentNode.setAttribute("stroke","");
strz.parentNode.setAttribute("stroke-width","");
} else {
var strID=strz.getAttribute("id").substr(0,pos+1);

}
tooltip.setAttribute("visibility","hidden");
}
}