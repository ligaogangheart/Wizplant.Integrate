var state;
var trans;
var attr_doc_type = ".htm";
var targetNode = null;

var ori_color; 
var bkori_color; 
var bsori_color; 

function highlight(evt){
	evt.target.setAttribute("fill-opacity","1.0");
	evt.target.setAttribute("stroke-opacity","1.0");
}

function unhighlight(evt){
	evt.target.setAttribute("fill-opacity","0.6");
	evt.target.setAttribute("stroke-opacity","0.6");
}

function ontext(evt,colorchange){
	var text0 = evt.target; 
	(colorchange==true)&&(text0.setAttribute("fill","rgb(255,0,0)"));
	ShowTooltip(evt,text0.getAttribute("alt"));
}

function outtext(evt,colorchange){
	var text0 = evt.target;
	(colorchange==true)&&(text0.setAttribute("fill","rgb(255,255,255)"));
	HideTooltip(evt);
}

function bs_mousemove(evt){ 
   var rect01 = evt.target; 
   bsori_color = rect01.getAttribute("class");
   
   rect01.setAttribute("class","buschoice"); 
}
function bs_mouseout(evt){ 
   var rect01 = evt.target; 
   rect01.setAttribute("class",bsori_color); 
}
function mousemove(evt){ 
   var rect01 = evt.target; 
   ori_color = rect01.getAttribute("class");
   
   if(ori_color.search(new RegExp("bk","i"))>-1)
	   rect01.setAttribute("class","bkchoice"); 
   else
   	   rect01.setAttribute("class","choice"); 
	   
	targetNode=evt.getTarget();
	if(targetNode==null) return;
	var use = targetNode;
	var tar=targetNode.getAttribute("xlink:href");
	var stringlen=tar.length;
	var symbol_type=tar.substring(1,13);
	if(symbol_type=="Disconnector")
	{
		var temp_tar=tar.substring(stringlen-5, stringlen); 
		
		switch(temp_tar)
		{
			case 'Close':
			        //update_menu('switch_menu','close');
				break;
			case '-Open':
				    //update_menu('switch_menu','unclose');
				break;
		}
	}
	else if(symbol_type=="Transformer:")
	{
			//update_menu('trans_menu','');
	}

}
function mouseout(evt){ 
   var rect01 = evt.target;
   rect01.setAttribute("class",ori_color);
    faire_menus('myCustomMenu');

	var use = targetNode;
	if(targetNode==null) return;

	var tar=targetNode.getAttribute("xlink:href");
}

function changetransform(matrix,dx,dy)
{
	 var transform_m = matrix;
	 var string_len;
   string_len =  transform_m.length;
   ///matrix(0.440000 0 0 0.294118 197.600006 410.352940) 
   var tfm_a;
   var tfm_b;
   var tfm_c;
   var tfm_d;
   var tfm_e;
   var tfm_f;
    var id1=transform_m.indexOf("matrix(");
    var id2=transform_m.indexOf(" ");
    if(id1<0)    //错误 
		return;
    tfm_a=transform_m.substring(7,id2);
    
    var text, text2;
    text = transform_m.substring(id2+1, string_len);
    id1=text.indexOf(" ");
    tfm_b =text.substring(0,id1);
 
    string_len =  text.length; 
    text2 = text.substring(id1+1, string_len);
    id2=text2.indexOf(" ");
    tfm_c =text2.substring(0,id2);

    string_len =  text2.length; 
    text = text2.substring(id2+1, string_len);
    id1=text.indexOf(" ");
    tfm_d =text.substring(0,id1);
   
    string_len =  text.length; 
    text2 = text.substring(id1+1, string_len);
    id2=text2.indexOf(" ");
    tfm_e =text2.substring(0,id2);

    string_len =  text2.length; 
    text = text2.substring(id2+1, string_len);
    id1=text.indexOf(")");
    tfm_f =text.substring(0,id1);
    
   
   // alert(tfm_a+","+tfm_b+","+tfm_c+","+tfm_d+","+tfm_e+","+tfm_f);
   
   var matrix_a=parseFloat(tfm_a);
   var matrix_b=parseFloat(tfm_b);
   var matrix_c=parseFloat(tfm_c);
   var matrix_d=parseFloat(tfm_d);
   var matrix_e=parseFloat(tfm_e)+parseFloat(dx);
   var matrix_f=parseFloat(tfm_f)+parseFloat(dy);
   if(matrix_a<1 && matrix_a>0)
   {
  	matrix_a=1.0;
   }else
   if(matrix_a>-1 && matrix_a<0)
   {
        matrix_a=-1.0;
   }
   if(matrix_d>-1 && matrix_d<0)
   { 
        matrix_d=-1.0;
   }else
   if(matrix_d<1 && matrix_d>0)
    {
         matrix_d=1.0;
    }
   return "matrix("+matrix_a+" "+matrix_b+" "+matrix_c+" "+matrix_d+" "+matrix_e+" "+matrix_f+")";
}

function tagup(evt){
//alert(targetNode);
	if(targetNode==null) return;

  	var rect01 = targetNode; 
	var tar=targetNode.getAttribute("xlink:href");
	var stringlen=tar.length;
	var symbol_type=tar;
  	var root=rect01.getParentNode(); 
  	var nodeid=root.getAttribute("id");
	var nodename=root.getAttribute("name");
	var laynode;
	laynode=root.getParentNode();
	if(nodeid==0)
	{
	 nodeid=root.getParentNode().getAttribute("id");
	 nodename=root.getParentNode().getAttribute("name");
	 laynode=root.getParentNode().getParentNode();
	}
	if(symbol_type.search(new RegExp("Transformer","i"))>-1)
	{
  	ttt=root.getParentNode().getAttribute("id");
	laynode=root.getParentNode().getParentNode();
	}
	var layname=laynode.getAttribute("id");

//	alert(targetNode.getParentNode().getAttribute("transform"));
   var transform_m = targetNode.getParentNode().getAttribute("transform");
//   alert(transform_m);
   var string_len;
   string_len =  transform_m.length;
   ///matrix(0.440000 0 0 0.294118 197.600006 410.352940) 
   var tfm_a;
   var tfm_b;
   var tfm_c;
   var tfm_d;
   var tfm_e;
   var tfm_f;
    var id1=transform_m.indexOf("matrix(");
    var id2=transform_m.indexOf(" ");
    if(id1<0)    //错误 
		return;
    tfm_a=transform_m.substring(7,id2);
    
    var text, text2;
    text = transform_m.substring(id2+1, string_len);
    id1=text.indexOf(" ");
    tfm_b =text.substring(0,id1);
 
    string_len =  text.length; 
    text2 = text.substring(id1+1, string_len);
    id2=text2.indexOf(" ");
    tfm_c =text2.substring(0,id2);

    string_len =  text2.length; 
    text = text2.substring(id2+1, string_len);
    id1=text.indexOf(" ");
    tfm_d =text.substring(0,id1);
   
    string_len =  text.length; 
    text2 = text.substring(id1+1, string_len);
    id2=text2.indexOf(" ");
    tfm_e =text2.substring(0,id2);

    string_len =  text2.length; 
    text = text2.substring(id2+1, string_len);
    id1=text.indexOf(")");
    tfm_f =text.substring(0,id1);
    
   
   // alert(tfm_a+","+tfm_b+","+tfm_c+","+tfm_d+","+tfm_e+","+tfm_f);
   
   var matrix_a=parseFloat(tfm_a);
   var matrix_b=parseFloat(tfm_b);
   var matrix_c=parseFloat(tfm_c);
   var matrix_d=parseFloat(tfm_d);
   var matrix_e=parseFloat(tfm_e);
   var matrix_f=parseFloat(tfm_f);
   
   //alert(matrix_a+","+matrix_b+","+matrix_c+","+matrix_d+","+matrix_e+","+matrix_f);

	getElementMatrix(targetNode);
	//alert( "["+MV[0]+" "+MV[1]+" "+MV[2]+" "+MV[3]+" "+MV[4]+" "+MV[5]+"]" );
	//alert(ret);
    var ref = targetNode.getAttribute("xlink:href").substr(1);
    
    var symbol = svgDocument.getElementById( ref );
    if( symbol == null ) return;
    var rects = symbol.getElementsByTagName("rect");
	if( rects.length<=0 ) return;

	var xx = rects.item(0).getAttribute( "x" );
	var yy = rects.item(0).getAttribute( "y" );
	var ww = rects.item(0).getAttribute( "width" );
    var hh = rects.item(0).getAttribute( "height" );
	
	//alert(xx+","+yy+","+ww+","+hh+",");
	
	//alert( "["+MV[0]+" "+MV[1]+" "+MV[2]+" "+MV[3]+" "+MV[4]+" "+MV[5]+"]" );
	
	//var x0  = matrix_a*xx+matrix_c*yy+matrix_e;
	//var y0  = matrix_b*xx+matrix_d*yy+matrix_f;
	
	var x = MV[0]*xx+MV[2]*yy+MV[4];
	var y = MV[1]*xx+MV[3]*yy+MV[5];

	//alert(x+","+y+","+ww+","+hh+",");
	
	try {    
    if (typeof(clientProcessor)=='undefined')
	;
	else
	clientProcessor.uptag(nodeid,x, y, 20, 20,"tag1");
	} 
	catch (e) {    
	;
	}
}


function tagdown(evt){
//alert(targetNode);
  	var rect01 = targetNode; 
	var tar=targetNode.getAttribute("xlink:href");
	var stringlen=tar.length;
	var symbol_type=tar;
  	var root=rect01.getParentNode(); 
  	var nodeid=root.getAttribute("id");
	var nodename=root.getAttribute("name");

	try{
	if (typeof(clientProcessor)=='undefined')
	;
	else
	clientProcessor.downtag(nodeid,"tag1");
	} 
	catch (e) {    
	;
	}

}

function test1(){
	if(targetNode==null) return;

	var rect01 = targetNode; 
	var tar=targetNode.getAttribute("xlink:href");
	var stringlen=tar.length;
	var symbol_type=tar;
	
	if(rect01.getParentNode().getAttribute("id") == "")
	  	var root=rect01.getParentNode().getParentNode(); 
	else
		var root=rect01.getParentNode();
		 
  	var nodeid=root.getAttribute("id");
	var nodename=root.getAttribute("name");

	alert(tar+","+nodeid+","+nodename+","+root.getAttribute("tablename")+","+root.getAttribute("colname"));
}

function test2(){
	if(targetNode==null) return;

	var rect01 = targetNode; 
	var tar=targetNode.getAttribute("xlink:href");
	var stringlen=tar.length;
	var symbol_type=tar;
	
	if(rect01.getParentNode().getAttribute("id") == "")
	  	var root=rect01.getParentNode().getParentNode(); 
	else
		var root=rect01.getParentNode();
		
  	var nodeid=root.getAttribute("id");
	var nodename=root.getAttribute("name");

	alert(nodeid+","+nodename);
}
function statdata(evt)
{
	//alert(targetNode);
	if(targetNode==null) return;

  	var rect01 = targetNode; 
	var tar=targetNode.getAttribute("xlink:href");
	var stringlen=tar.length;
	var symbol_type=tar;
  	var root=rect01.getParentNode(); 
  	var nodeid=root.getAttribute("id");
	var nodename=root.getAttribute("name");
	var laynode;
	laynode=root.getParentNode();
	if(nodeid==0 || nodeid=="")
	{
	 nodeid=root.getParentNode().getAttribute("id");
	 nodename=root.getParentNode().getAttribute("name");
	 laynode=root.getParentNode().getParentNode();
	}
	if(symbol_type.search(new RegExp("Transformer","i"))>-1)
	{
	 ttt=root.getParentNode().getAttribute("id");
	 laynode=root.getParentNode().getParentNode();
	}
	
	var layname=laynode.getAttribute("id");

	try{
	if (typeof(clientProcessor)=='undefined')
	;
	else
	clientProcessor.callEvent(nodename,nodeid,"statdata");
	} 
	catch (e) {    
	;
	}
}

function attr(evt){
//alert(targetNode);
	if(targetNode==null) return;
  var rect01 = targetNode; 
	var tar=targetNode.getAttribute("xlink:href");
	var stringlen=tar.length;
	var symbol_type=tar;
  var root=rect01.getParentNode(); 
  var nodeid=root.getAttribute("id");
	var nodename=root.getAttribute("name");
	var laynode;
	laynode=root.getParentNode();
	if(nodeid==0 || nodeid=="")
	{
	 nodeid=root.getParentNode().getAttribute("id");
	 nodename=root.getParentNode().getAttribute("name");
	 laynode=root.getParentNode().getParentNode();
	}
	if(nodename="" || nodename==0 ){
		var parentNode = root.getParentNode();
		//var nodesArray = parentNode.getElementsByTagName("metadata");
	 	var metadatanodes = root.getElementsByTagName("cge:PSR_Ref");
		var temp_nodename=metadatanodes.item(0).getAttribute("ObjectName");

		nodename=temp_nodename.substring(temp_nodename.search(new RegExp("-","i"))+4,temp_nodename.length);
		alert(nodename);
		
	}
	if(symbol_type.search(new RegExp("Transformer","i"))>-1)
	{
	 ttt=root.getParentNode().getAttribute("id");
	 laynode=root.getParentNode().getParentNode();
	}
	var layname=laynode.getAttribute("id");

	try{
	if (typeof(clientProcessor)=='undefined')
	;
	else{
		clientProcessor.callEvent(nodename,nodeid,"devprop");
	}
	} 
	catch (e) {    
	;
	}

}

function transformer_attr(evt){
//alert(targetNode);
	if(targetNode==null) return;

  var rect01 = targetNode; 
	var tar=targetNode.getAttribute("xlink:href");
	var stringlen=tar.length;
	var symbol_type=tar;
  var root=rect01.getParentNode(); 
  var nodeid=root.getAttribute("id");
	var nodename=root.getAttribute("name");
	var laynode;
	laynode=root.getParentNode();
	
	if(nodeid==0 || nodeid=="" || nodeid.search(new RegExp("WD-"),"i")>-1)
	{
	 nodeid=root.getParentNode().getAttribute("id");
	 nodename=root.getParentNode().getAttribute("name");
	 laynode=root.getParentNode().getParentNode();
	}
	if(nodename="" || nodename==0 ){
		var parentNode = root.getParentNode();
		//var nodesArray = parentNode.getElementsByTagName("metadata");
	 	var metadatanodes = parentNode.getElementsByTagName("cge:PSR_Ref");
		var temp_nodename=metadatanodes.item(0).getAttribute("ObjectName");
		nodename=temp_nodename.substring(temp_nodename.search(new RegExp("-","i"))+1,temp_nodename.length);
		//alert(nodename);
	}

	nodename=nodeid.substring(nodeid.search(new RegExp("-","i"))+1, nodeid.length);

	if(symbol_type.search(new RegExp("Transformer","i"))>-1)
	{
	 ttt=root.getParentNode().getAttribute("id");
	 laynode=root.getParentNode().getParentNode();
	}
	var layname=laynode.getAttribute("id");

	try{
	if (typeof(clientProcessor)=='undefined')
	;
	else{
		clientProcessor.callEvent(nodename,nodeid,"devprop");
	}
	} 
	catch (e) {    
	;
	}

}

function statattr(evt){
		 	
//alert(targetNode);
	if(targetNode==null) return;

  	var rect01 = targetNode; 
	var tar=targetNode.getAttribute("xlink:href");
	var stringlen=tar.length;
	var symbol_type=tar;
  	var root=rect01.getParentNode(); 
  	var nodeid=root.getAttribute("id");
	var nodename=root.getAttribute("name");
	var laynode;
	laynode=root.getParentNode();
	if(nodeid==0 || nodeid=="")
	{
	 nodeid=root.getParentNode().getAttribute("id");
	 nodename=root.getParentNode().getAttribute("name");
	 laynode=root.getParentNode().getParentNode();
	}
	if(symbol_type.search(new RegExp("Transformer","i"))>-1)
	{
	 ttt=root.getParentNode().getAttribute("id");
	 laynode=root.getParentNode().getParentNode();
	}
	
	var layname=laynode.getAttribute("id");

	try{
	if (typeof(clientProcessor)=='undefined')
	;
	else
	clientProcessor.callEvent(nodename,nodeid,"statdata");
	} 
	catch (e) {    
	;
	}

}

function scadaattr(evt){
//alert(targetNode);
	if(targetNode==null) return;

  	var rect01 = targetNode; 
	var tar=targetNode.getAttribute("xlink:href");
	var stringlen=tar.length;
	var symbol_type=tar;
  	var root=rect01.getParentNode(); 
  	var nodeid=root.getAttribute("id");
	var nodename=root.getAttribute("name");
	var laynode;
	laynode=root.getParentNode();
	if(nodeid==0 || nodeid=="")
	{
	 nodeid=root.getParentNode().getAttribute("id");
	 nodename=root.getParentNode().getAttribute("name");
	 laynode=root.getParentNode().getParentNode();
	}
	if(symbol_type.search(new RegExp("Transformer","i"))>-1)
	{
	 ttt=root.getParentNode().getAttribute("id");
	 laynode=root.getParentNode().getParentNode();
	}
	
	var layname=laynode.getAttribute("id");

	try{
	if (typeof(clientProcessor)=='undefined')
	;
	else
	clientProcessor.callEvent(nodename,nodeid,"scadaprop");
	} 
	catch (e) {    
	;
	}

}

function break_mousemove(evt){

	//alert("break_mousemove");
    var rect01 = evt.target; 
    bkori_color = rect01.getAttribute("class");
   
    rect01.setAttribute("class","bkchoice"); 
	targetNode=evt.getTarget();
	if(targetNode==null) return;

	var use = targetNode;
	var tar=targetNode.getAttribute("xlink:href");
	//var stringlen=tar.length;
	//var temp_tar=tar.substring(stringlen-5, stringlen); 
	
	if(tar.search(new RegExp("Close","i"))>-1){
		//update_menu('breaker_menu','close');
	}
	else if(tar.search(new RegExp("Open","i"))>-1){
		//update_menu('breaker_menu','unclose');
	}
	
}
function break_mouseout(evt){
   //alert("break_mouseout");
    var rect01 = evt.target; 
	
    rect01.setAttribute("class",bkori_color); 
	faire_menus('myCustomMenu');

	var use = targetNode;
	if(targetNode==null) return;

	var tar=targetNode.getAttribute("xlink:href");
}


/*

function attr(evt){
	var type;
	
	var tar=targetNode.getAttribute("xlink:href");
	switch(tar)
	{
		case '#breaker_c':
		        
		case '#breaker_uc':
			type = "cim:Breaker";
			break;
		case '#switch_c':
			
		case '#switch_uc':
			type = "cim:Disconnector";
			break;
		case '#bus':
			type = "cim:BusbarSection";
			break;
		case '#winding':
		case '#winding_c':
			type = "cim:TransformerWinding";
			break;
		case '#reactor':
			type = "cim:Compensator";
			break;
			
	}
	//top.document.title="equ";
//	var equ = evt.getTarget();
	var refer = targetNode.getAttribute("id");
	

        var info=importXML(refer,type);
//      var switchSt=getSwitchSt(refer);
	var win = open("", "", 
	"top=200 left=200 toolbar=no, location=no, directories=no, status=no, menubar=yes, scrollbars=yes, resizable=yes, copyhistory=yes,width=500,height=300");

	win.document.write("<html><head><title>设备属性</title></head><body>"
				+"<table border><caption align=top>设备属性</caption> <tr><th>rdf:ID</th> <td>"+refer+"</td>"+info+"</table></body>");

}
*/
var layerstate = new Array(1, 1, 1, 1, 1, 1, 1);
var layername = new Array("breaker", "switch", "bus", "line", "transformer", "other", "text");

function layercontrol(n){
	var obj = svgDocument.getElementById(layername[n]);
	if(layerstate[n] != 0){
		obj.setAttribute("display", "none");
		layerstate[n] = 0;
	}else{
		obj.setAttribute("display","");
		layerstate[n] = 1;
	}
}
function clickrect(evt,n)
{
	var rect = evt.getTarget();
	if(layerstate[n] != 0 ){
		rect.setAttribute("fill", "black");
		layercontrol(n);
	}else {
		rect.setAttribute("fill", "red");
        	layercontrol(n);
	}
}
//get the data of cim and display it in svg

var timer;
var obj;

var color=0;


function update(evt)
{
/*	var closeSW = get_cSw();
	var tarId;
	var objects,st;
	
	for(i=0; closeSW.length; i++)
	{
		tarId = closeSW[i];
		if(tarId==' '){
		break;}else{
			//alert(transId(tarId));
		       
			objects = svgDocument.getElementById(tarId);
			if(objects.getAttribute("xlink:href") == "#breaker_c" || objects.getAttribute("xlink:href") == "#breaker_uc")
				objects.setAttribute("xlink:href","#breaker_c");
			else if(objects.getAttribute("xlink:href") == "#switch_c" || objects.getAttribute("xlink:href") == "#switch_uc")
				objects.setAttribute("xlink:href","#switch_c");
			}
	}
	var uncloseSW = get_ucSw();
	var tarId;
	var objects,st;
	
	for(i=0; uncloseSW.length; i++)
	{
		tarId = uncloseSW[i];
		if(tarId==' '){
		break;}else{
			//alert(transId(tarId));
		       
			objects = svgDocument.getElementById(tarId);
			if(objects.getAttribute("xlink:href") == "#breaker_c" || objects.getAttribute("xlink:href") == "#breaker_uc")
				objects.setAttribute("xlink:href","#breaker_uc");
			else if(objects.getAttribute("xlink:href") == "#switch_c" || objects.getAttribute("xlink:href") == "#switch_uc")
				objects.setAttribute("xlink:href","#switch_uc");
			}
	}*/
	data_update();	
	timer = setInterval("data_update()", 5000);
	
}
function data_update()
{	
	/*switchs = get_switch();
//	alert(switchs);
	var closeSW = get_cSw(switchs);
	var tarId;
	var objects,st;
	
	for(i=0; i<closeSW.length; i++)
	{
		tarId = closeSW[i];
		//alert(transId(tarId));
		if(tarId==' '){
		break;}else{
			//alert(transId(tarId));
		       
			objects = svgDocument.getElementById(transId(tarId));
			if(objects.getAttribute("xlink:href") == "#breaker_c" || objects.getAttribute("xlink:href") == "#breaker_uc")
				objects.setAttribute("xlink:href","#breaker_c");
			else if(objects.getAttribute("xlink:href") == "#switch_c" || objects.getAttribute("xlink:href") == "#switch_uc")
				objects.setAttribute("xlink:href","#switch_c");
			}
	}
	var uncloseSW = get_ucSw(switchs);
	var tarId;
	var objects,st;
	
	for(i=0; i<uncloseSW.length; i++)
	{
		tarId = uncloseSW[i];
		if(tarId==' '){
		break;}else{
			//alert(transId(tarId));
		       
			objects = svgDocument.getElementById(transId(tarId));
			if(objects.getAttribute("xlink:href") == "#breaker_c" || objects.getAttribute("xlink:href") == "#breaker_uc")
				objects.setAttribute("xlink:href","#breaker_uc");
			else if(objects.getAttribute("xlink:href") == "#switch_c" || objects.getAttribute("xlink:href") == "#switch_uc")
				objects.setAttribute("xlink:href","#switch_uc");
			}
	}
	
	var meas = get_measArray();
	var newvalue = dataUp(meas);
	
	for(i=0; i<meas.length; i++){
		if(meas[i] != ' '){
		var objects = svgDocument.getElementById(meas[i]);
		var newText = svgDocument.createTextNode(newvalue[i]);
		objects.replaceChild(newText, objects.getFirstChild());
	}
	}
*/	
}


function get_measArray(){
	var objects = svgDocument.getElementsByTagName("text");
	var strings = ' ';
	var meas = new Array(500);
	for(i=0;i<500;i++)
	meas[i]= ' ';
	var j = 0;
	for(i=0; i<objects.length; i++){
		if((strings=objects.item(i).getAttribute("id"))!= 0)
		{
			meas[j]=strings;
			j++;
			}
	}
	objects = svgDocument.getElementsByTagName("tspan");
	for(i=0; i<objects.length; i++){
		if((strings=objects.item(i).getAttribute("id"))!= 0)
		{
			meas[j] = strings;
			j++;
			}
			}
	return meas;
}
function get_switch(){
	
	var switchs = new Array(500);
	for(i=0;i<500;i++)
	switchs[i]= ' ';
	var j = 0;
	var tarid;
	var objects = svgDocument.getElementsByTagName("use");
//	alert(objects.item(0).nodeValue);
	for(i=0; i<objects.length; i++){
		var sw_symbol=objects.item(i).getAttribute("xlink:href");
		var string_len=sw_symbol.length;
		var temp_tar=sw_symbol.substring(1,8);
		var temp_tar2=sw_symbol.substring(1,13);
		if(temp_tar=="Breaker"||temp_tar2=="Disconnector")
		{  
		if((tarid=objects.item(i).getParentNode().getParentNode().getAttribute("id"))!= 0)
		{switchs[j]=tarid;
		j++;
		}
	}
	}
	
	return switchs;
}
function stopupdate(evt)
{
	clearInterval(timer);
}

function initialize(evt)
{
	//faire_menus('myCustomMenu');
	//data_update();
	
	initialize1(evt);
		
	try{
	if (typeof(clientProcessor)=='undefined'){
	
	dfi_initialize(evt);}
	else
	{
		
	 clientProcessor.initVisualSet(evt, Graph_BgColor);
	}
	
	} 
	catch (e) {    
	;
	}

}

function changeCanvasBkColor(bgcolor){
	if (typeof(application)=='undefined')
		return;
		
	var __svgid = oo("svgcanvas");
	var __svgdoc = oo("svgcanvas").getSVGDocument();
    var contourDoc = __svgdoc;
	
	var headlayer = contourDoc.getElementById("headlayer");
    var texts;
    if( headlayer != null ){
        texts = headlayer.getElementsByTagName("rect");
    }
    else{
        texts = contourDoc.getElementsByTagName("rect");
    }
	
    var len = texts.length;
    if( len>0 ){
        //用一新组保存原来的颜色
        var ori = contourDoc.getElementById("noContourOriColor");
        if( ori == null ){
            var obj = this.visualSvgDoc.createElement( "g" );
            obj.setAttribute( "id", "noContourOriColor" );
            obj.setAttribute( "fill", texts.item(0).getAttribute("fill") );
            var svgs = contourDoc.getElementsByTagName("svg");
            svgs.item(0).appendChild( obj );
        }

        texts.item(0).getStyle().setProperty( "stroke", bgcolor );
        texts.item(0).getStyle().setProperty( "fill", bgcolor );
    }
  	
	__svgid.setBackColor(bgcolor);
}

function faire_menus(nom_menu){
	var newMenuRoot=parseXML(printNode(document.getElementById(nom_menu)),contextMenu);
	contextMenu.replaceChild(newMenuRoot,contextMenu.firstChild);
}
function curve(evt)
{
	if(targetNode==null) return;

  	var rect01 = targetNode; 
	var tar=targetNode.getAttribute("xlink:href");
	var stringlen=tar.length;
	var symbol_type=tar;
  	var root=rect01.getParentNode(); 
  	var nodeid=root.getAttribute("id");
	var nodename=root.getAttribute("name");
	var laynode;
	laynode=root.getParentNode();
	if(nodeid==0 || nodeid=="")
	{
	 nodeid=root.getParentNode().getAttribute("id");
	 nodename=root.getParentNode().getAttribute("name");
	 laynode=root.getParentNode().getParentNode();
	}
	if(symbol_type.search(new RegExp("Transformer","i"))>-1)
	{
	 ttt=root.getParentNode().getAttribute("id");
	 laynode=root.getParentNode().getParentNode();
	}
	var layname=laynode.getAttribute("id");

	try{
	if (typeof(clientProcessor)=='undefined')
	;
	else
	clientProcessor.callEvent(nodename,nodeid,"scadarealcurve");
	} 
	catch (e) {    
	;
	}
}
function morecurves(evt)
{
	if(targetNode==null) return;

  	var rect01 = targetNode; 
	var tar=targetNode.getAttribute("xlink:href");
	var stringlen=tar.length;
	var symbol_type=tar;
  	var root=rect01.getParentNode(); 
  	var nodeid=root.getAttribute("id");
	var nodename=root.getAttribute("name");
	var laynode;
	laynode=root.getParentNode();
	if(nodeid==0 || nodeid=="")
	{
	 nodeid=root.getParentNode().getAttribute("id");
	 nodename=root.getParentNode().getAttribute("name");
	 laynode=root.getParentNode().getParentNode();
	}
	if(symbol_type.search(new RegExp("Transformer","i"))>-1)
	{
	 ttt=root.getParentNode().getAttribute("id");
	 laynode=root.getParentNode().getParentNode();
	}
	var layname=laynode.getAttribute("id");

	try{
	if (typeof(clientProcessor)=='undefined')
	;
	else
	clientProcessor.callEvent(nodename,nodeid,"scadamorecurves");
	} 
	catch (e) {    
	;
	}
}

function showAbout(evt){}
function GetElement(evt)
{
	targetNode=evt.getTarget();
}
function update_menu(menu_id, states)
{
	var targetItem;
        var optionValue;
        var eachItem;
        var eachSubItem;
        var subMenuItem;
        var menuState;
        
        //get reference to the currentMenu in use
      	var menuItems = contextMenu.childNodes.item(0).childNodes;
        for(i=0; i<menuItems.length; i++){
        	if(menuItems.item(i).nodeName == 'menu'){
        		eachItem = menuItems.item(i);
        		if(eachItem.getAttribute('id') == menu_id){
        			eachItem.setAttribute('display', 'inline');
        			subMenuItem = eachItem.childNodes;
        			if(states == 'close'){
        			for(j=0; j<subMenuItem.length; j++)
        			{
        				eachSubItem = subMenuItem.item(j);
        				if (1 == eachSubItem.nodeType){
        					menuState = eachSubItem.getAttribute("id");
        					if(menuState=='me_c')
        						eachSubItem.setAttribute('enabled','no');        					
        				}	        			
        			}
        			}
        			if(states == 'unclose'){
        		        for(j=0; j<subMenuItem.length; j++)
        			{
        				eachSubItem = subMenuItem.item(j);
        				if (1 == eachSubItem.nodeType){
        					menuState = eachSubItem.getAttribute("id");
        					if(menuState=='me_uc')
        						eachSubItem.setAttribute('enabled','no');        					
        				}	        			
        			}
        			}
        		}       			
        	}    	
	}
}	

function closewitch()
{
	if(targetNode==null) return;

	var tar=targetNode.getAttribute('xlink:href');
	
	var string_len=tar.length;
	
	if(tar.search(new RegExp("-Close","i"))>=0) return;
	
	var temp_tar=tar.substring(0,tar.search(new RegExp("-","i")));
	
	var temp_tar2=temp_tar+"-Close";
	targetNode.setAttribute('xlink:href', temp_tar2);

	if(tar.search(new RegExp("breaker|Breaker","i"))>-1)
	{
		var kvcolor=targetNode.getParentNode().getAttribute('class');
		temp_tar="bk"+kvcolor;
		targetNode.getParentNode().setAttribute('class', temp_tar);
	}
}
function openwitch()
{
	if(targetNode==null) return;

	var tar=targetNode.getAttribute('xlink:href');
	var string_len=tar.length;
	
	if(tar.search(new RegExp("-Open","i"))>=0) return;
	
	var temp_tar=tar.substring(0,tar.search(new RegExp("-","i")));
	
	var temp_tar2=temp_tar+"-Open";
	targetNode.setAttribute('xlink:href', temp_tar2);
	
	if(tar.search(new RegExp("breaker|Breaker","i"))>-1)
	{
		var kvcolor=targetNode.getParentNode().getAttribute('class');
		string_len=kvcolor.length;
		temp_tar=kvcolor.substring(2,string_len);
		targetNode.getParentNode().setAttribute('class', temp_tar);
	}
}

function closeswitch()
{
	if(targetNode==null) return;
	var tar=targetNode.getAttribute('xlink:href');
	
	var string_len=tar.length;
	
	if(tar.search(new RegExp("-Close","i"))>=0) return;
	
	var temp_tar=tar.substring(0,tar.search(new RegExp("-","i")));
	
	var temp_tar2=temp_tar+"-Close";
	targetNode.setAttribute('xlink:href', temp_tar2);

	if(tar.search(new RegExp("breaker|Breaker","i"))>-1)
	{
		var kvcolor=targetNode.getParentNode().getAttribute('class');
		temp_tar="bk"+kvcolor;
		targetNode.getParentNode().setAttribute('class', temp_tar);
	}
}
function openswitch()
{
	if(targetNode==null) return;

	var tar=targetNode.getAttribute('xlink:href');
	var string_len=tar.length;
	
	if(tar.search(new RegExp("-Open","i"))>=0) return;
	
	var temp_tar=tar.substring(0,tar.search(new RegExp("-","i")));
	
	var temp_tar2=temp_tar+"-Open";
	targetNode.setAttribute('xlink:href', temp_tar2);
	
	if(tar.search(new RegExp("breaker|Breaker","i"))>-1)
	{
		var kvcolor=targetNode.getParentNode().getAttribute('class');
		string_len=kvcolor.length;
		temp_tar=kvcolor.substring(2,string_len);
		targetNode.getParentNode().setAttribute('class', temp_tar);
	}
}

function paint(evt)
{
	var tarId;
	var objects,st;
	var energizedNodes = new Array(500);
	for(i=0;i<500;i++)
	energizedNodes[i]= ' ';
	energizedNodes = getToponode();
	var uenNodes = getUeToponode();
	for(i=0; i<energizedNodes.length; i++)
	{
		tarId = energizedNodes[i];
		if(tarId==' '){
		break;}else{
			//alert(transId(tarId));
		       
			objects = svgDocument.getElementById(transId2(tarId));
			st = objects.getAttribute("class");
			//alert(st);
			if(st=="l35"){
			objects.setAttribute("class","kv35");}else{
			if(st=="l220"){
			objects.setAttribute("class","kv220");}else{
			if(st=="l500"){
			objects.setAttribute("class","kv500");}
			}}
		}
	}for(i=0; i<uenNodes.length; i++)
	{
		tarId = uenNodes[i];
		if(tarId==' '){
		break;}else{
			//alert(transId(tarId));
		       
			objects = svgDocument.getElementById(transId2(tarId));
			st = objects.getAttribute("class");
			//alert(st);
			if(st=="kv35"){
			objects.setAttribute("class","l35");}else{
			if(st=="kv220"){
			objects.setAttribute("class","l220");}else{
			if(st=="kv500"){
			objects.setAttribute("class","l500");}
			}}
		}
	}
	
	var ueEqu = new Array(500);
	for(i=0;i<500;i++)
	ueEqu[i]= ' ';
	ueEqu = get_ue_equ();
	//alert(ueEqu);
	var enerEqu = new Array(500);
	for(i=0;i<500;i++)
	enerEqu[i] = ' ';
	enerEqu = get_e_equ();
	//alert(enerEqu);
	for(i=0; i<ueEqu.length; i++)
	{
		tarId = ueEqu[i];
		if(tarId==' '){
		break;}else{
			//alert(transId(tarId));
		       
			objects = svgDocument.getElementById(tarId);
			st = objects.getAttribute("class");
			//alert(st);
			if(st=="kv35"){
			objects.setAttribute("class","l35");}else{
			if(st=="kv220"){
			objects.setAttribute("class","l220");}else{
			if(st=="kv500"){
			objects.setAttribute("class","l500");}
			}}
		}
	}
	for(i=0; i<enerEqu.length; i++)
	{
		tarId = enerEqu[i];
		if(tarId==' '){
		break;}else{
			
		       
			objects = svgDocument.getElementById(tarId);
			st = objects.getAttribute("class");
			//alert(st);
			if(st=="l35"){
			objects.setAttribute("class","kv35");}else{
			if(st=="l220"){
			objects.setAttribute("class","kv220");}else{
			if(st=="l500"){
			objects.setAttribute("class","kv500");}
			}}
		}
	}
}
function transId(tarId)
{
	var newId;
	return newId = tarId.substring(0,17);
}
function transId2(tarId)
{
	var newId;
	return newId = tarId.substring(1,16);
}

function judge_mouse(evt,mn)
{ 
  if(evt.button==2)
  {
      var newMenuRoot = parseXML( printNode( document.getElementById(mn)), contextMenu ); 
      contextMenu.replaceChild( newMenuRoot.firstChild, contextMenu.firstChild ); 
  }
  else if(evt.button==0)
  {
	  var canvas = oo("svgcanvas");
	  canvas._svg_mouse_begin = new Array(evt.getClientX(),evt.getClientY());
	  //alert(evt.getClientX()+","+evt.getClientY());
  }
}

function judge_mousemove(evt)
{
/*	var moveline = application.getComponentById("svgcanvas")._embedobj.getSVGDocument().getElementById("move_line");
	if(moveline!=null){
	var canvas = application.getComponentById("svgcanvas");
	var begin = canvas.getWorldPoint(canvas._svg_mouse_begin);
	moveline.setAttribute( "x1", begin[0] );
    moveline.setAttribute( "y1", begin[1] );
	var end = canvas.getWorldPoint(new Array(evt.getClientX(),evt.getClientY()));
    moveline.setAttribute( "x2", end[0] );
    moveline.setAttribute( "y2", end[1] );
	}*/
}
function judge_mousedown(evt,mn)
{ 
  if(evt.button==2)
  {
      var newMenuRoot = parseXML( printNode( document.getElementById(mn)), contextMenu ); 
      contextMenu.replaceChild( newMenuRoot.firstChild, contextMenu.firstChild ); 
  }
  else if(evt.button==0)
  {
	  if(typeof(application)!='undefined' || application!=null){
	  var canvas = oo("svgcanvas");
	  canvas._svg_mouse_begin = new Array(evt.getClientX(),evt.getClientY());
	 }
	//alert(evt.getClientX()+","+evt.getClientY());
  }
}
function judge_mouseup(evt,mn)
{ 
  if(evt.button==0)
  {
	 if(typeof(application)!='undefined' || application!=null){
	 var canvas = oo("svgcanvas");
	 canvas._svg_mouse_end = new Array(evt.getClientX(),evt.getClientY());
	 var x2 = (canvas._svg_mouse_end[0]-canvas._svg_mouse_begin[0])*(canvas._svg_mouse_end[0]-canvas._svg_mouse_begin[0]);
	 var y2 = (canvas._svg_mouse_end[1]-canvas._svg_mouse_begin[1])*(canvas._svg_mouse_end[1]-canvas._svg_mouse_begin[1]);
	 if(Math.sqrt(x2+y2)<10 || canvas==null)
	 	return;
	 //alert(evt.getClientX()+","+evt.getClientY());
	 canvas.dragControl();
	 //alert("dd2");
	 /*
	 var __svgdoc = application.getComponentById("svgcanvas")._embedobj.getSVGDocument();
	 var operlayer = __svgdoc.getElementById("operLayer");
	  
	 var moveline = __svgdoc.getElementById("move_line");
	 //if(moveline!=null)
	 //operlayer.removeChild(moveline);
	 */
	}
  }
}

function sqRt(form) {
var number = form.num.value;
var ans = Math.sqrt(number);
form.answer.value = ans;
}
function sqIt(form) {
var number = form.num.value;
var ans = eval(number * number);
form.answer.value = ans;
}



/***********************************************
**  HMH ADD FOR VISUAL SCRIPT
***********************************************/
/*
**  取得一个对象的矩阵变换
*/

var MV = new Array( 1., 0., 0., 1., 0., 0. );
function getElementMatrix( obj )
{
    MV[0] = 1.;
    MV[1] = 0.;
    MV[2] = 0.;
    MV[3] = 1.;
    MV[4] = 0.;
    MV[5] = 0.;

    var parent = obj;
    if( parent == null ){
        return;
    }
    var chr;
    var farray = new Array();
    while( parent != null ){
        if( parent.getNodeName() == "svg" ) break;

        var transformStr = parent.getAttribute("transform");
        if( transformStr == null ){
            parent = parent.getParentNode();
            continue;
        }
        
        var tempm = new Array( 1, 0, 0, 1, 0, 0 ); // 初始化当前节点矩阵
        //matrix translate scale rotate skewX skewY
        var len = transformStr.length;
        var idx = 0;
        var type;
        while(idx<len){
            // 下面假定正确的坐标变换
            type = 0;
            if( transformStr.charAt(idx) == 'm' ){ //matrix
                type = 1;
            }else
            if( transformStr.charAt(idx) == 't' ){ //translate
                type = 2;
            }else
            if( transformStr.charAt(idx) == 'r' ){ //rotate
                type = 3;
            }else
            if( transformStr.charAt(idx) == 's' ){ //scale skewX skewY
                if( transformStr.charAt(idx+1) == 'c' ) //scale
                    type = 4;
                else
                if( transformStr.charAt(idx+4) == 'X' ) //skewX
                    type = 5;
                else //skewY
                    type = 6;
            }else{
                idx++;
                continue;
            }
            
            var sub = transformStr.substring(idx);
            var epos = sub.indexOf(")");
            if( epos<0 ) return; // 错误
            idx += epos+1;
            epos--;
            while( sub.charAt(epos) == ' ' || sub.charAt(epos) == '\t' ) epos--;// 过滤掉末端空格
            var bpos = sub.indexOf("(");
            if( bpos<0 ) return; // 错误
            bpos++;
            while( sub.charAt(bpos) == ' ' || sub.charAt(bpos) == '\t' ) bpos++;// 过滤掉首端空格
            
            var temp0 = sub.substr( bpos, epos-bpos+1 ); //注意substr 与substring的区别
            //alert( "parser transform=\""+temp0+"\"" );

    //取数值farray[0]
            farray[0] = parseFloat(temp0);
            if( type == 5 ){ //skewX(一个参数)
                /*skewX(a)
                  [ 1 tan(a) 0 ]
                  [ 0   1    0 ]
                  [ 0   0    1 ]
                */
                var val = Math.tan(farray[0]);
                tempm[2] += tempm[0]*val;
                tempm[3] += tempm[1]*val;
                continue;
            }else
            if( type == 6 ){ //skewY(一个参数)
                /*skewY(a)
                  [ 1      0  0 ]
                  [ tan(a) 1  0 ]
                  [ 0      0  1 ]
                */
                var val = Math.tan(farray[0]);
                tempm[0] += tempm[2]*val;
                tempm[1] += tempm[3]*val;
                continue;
            }
            
    //取数值farray[1]
            var tidx1 = temp0.indexOf( " " ); // 这里假定无仅仅'\t'分隔
            var tidx2 = temp0.indexOf( "," );
            if( tidx1<0 && tidx2<0 ){
                if( type == 2 ){
                    /*translate(tx) = translate(tx, ty=0)
                    [1 0 tx]
                    [0 1 ty]
                    [0 0 1 ]
                    */
                    tempm[4] += tempm[0]*farray[0];
                    tempm[5] += tempm[1]*farray[0];
                }else
                if( type == 3 ){
                    /*rotate(a)
                    [ cos(a) -sin(a) 0 ]
                    [ sin(a)  cos(a) 0 ]
                    [   0      0     1 ]
                    */
                    //[ tempm[0]*cos(a)+tempm[2]*sin(a) -tempm[0]*sin(a)+tempm[2]*cos(a) tempm[4]
                    //[ tempm[1]*cos(a)+tempm[3]*sin(a) -tempm[1]*sin(a)+tempm[3]*cos(a) tempm[5]
                    var ttt = new Array(4);
                    ttt[0] = tempm[0]*cos(farray[0])+tempm[2]*sin(farray[0]);
                    ttt[1] = tempm[1]*cos(farray[0])+tempm[3]*sin(farray[0]);
                    ttt[2] = -tempm[0]*sin(farray[0])+tempm[2]*cos(farray[0]);
                    ttt[3] = -tempm[1]*sin(farray[0])+tempm[3]*cos(farray[0]);
                    for( var i=0; i<4; i++ ) tempm[i] = ttt[i];
                    
                }else
                if( type == 4 ){
                    /*scale(sx) = scale(sx, sy=sx)
                    [ sx  0  0 ]
                    [ 0  sy  0 ]
                    [ 0   0  1 ]
                    */
                    tempm[0] *= farray[0];
                    tempm[1] *= farray[0];
                    tempm[2] *= farray[0];
                    tempm[3] *= farray[0];
                }
                continue;
            }
            var tidx;
            if( tidx1>=0 ){
                tidx = tidx1;
                if( tidx2>=0 && tidx2<tidx1 ) tidx = tidx2;
            }
            else
                tidx = tidx2;
            tidx++;
            while( temp0.charAt(tidx) == ' ' || temp0.charAt(tidx) == '\t' ) tidx++;// 过滤掉首端空格
            var temp1 = temp0.substr( tidx );
            farray[1] = parseFloat(temp1);
            
            if( type == 4 ) { //scale
                /*scale(sx, sy)
                [ sx  0  0 ]
                [ 0  sy  0 ]
                [ 0   0  1 ]
                */
                tempm[0] *= farray[0];
                tempm[1] *= farray[0];
                tempm[2] *= farray[1];
                tempm[3] *= farray[1];
                continue;
            }else
            if( type == 2 ){ //translate
                /*translate(tx, ty)
                [1 0 tx]
                [0 1 ty]
                [0 0 1 ]
                */
                tempm[4] += tempm[0]*farray[0]+tempm[2]*farray[1];
                tempm[5] += tempm[1]*farray[0]+tempm[3]*farray[1];
                continue;
            }
            
    //取数值farray[2]
            tidx1 = temp1.indexOf( " " ); // 这里假定无仅仅'\t'分隔
            tidx2 = temp1.indexOf( "," );
            if( tidx1<0 && tidx2<0 ){
                continue;
            }
            if( tidx1>=0 ){
                tidx = tidx1;
                if( tidx2>=0 && tidx2<tidx1 ) tidx = tidx2;
            }
            else
                tidx = tidx2;
            tidx++;
            while( temp1.charAt(tidx) == ' ' || temp1.charAt(tidx) == '\t' ) tidx++;// 过滤掉首端空格
            var temp2 = temp1.substr( tidx );
            farray[2] = parseFloat(temp2);
            
            if( type == 3 ){
            //rotate(a, cx, cy ) = translate(cx, cy) rotate(a) translate(-cx, -cy)
                tempm[4] += tempm[0]*farray[0]+tempm[2]*farray[1];
                tempm[5] += tempm[1]*farray[0]+tempm[3]*farray[1];
            
                var ttt = new Array(4);
                ttt[0] = tempm[0]*cos(farray[0])+tempm[2]*sin(farray[0]);
                ttt[1] = tempm[1]*cos(farray[0])+tempm[3]*sin(farray[0]);
                ttt[2] = -tempm[0]*sin(farray[0])+tempm[2]*cos(farray[0]);
                ttt[3] = -tempm[1]*sin(farray[0])+tempm[3]*cos(farray[0]);
                for( var i=0; i<4; i++ ) tempm[i] = ttt[i];

                tempm[4] += -tempm[0]*farray[0]-tempm[2]*farray[1];
                tempm[5] += -tempm[1]*farray[0]-tempm[3]*farray[1];

                continue;
            }
        
    //取数值farray[3]
            tidx1 = temp2.indexOf( " " ); // 这里假定无仅仅'\t'分隔
            tidx2 = temp2.indexOf( "," );
            if( tidx1<0 && tidx2<0 ){
                continue;
            }
            if( tidx1>=0 ){
                tidx = tidx1;
                if( tidx2>=0 && tidx2<tidx1 ) tidx = tidx2;
            }
            else
                tidx = tidx2;
            tidx++;
            while( temp2.charAt(tidx) == ' ' || temp2.charAt(tidx) == '\t' ) tidx++;// 过滤掉首端空格
            var temp3 = temp2.substr( tidx );
            farray[3] = parseFloat(temp3);
    
    //取数值farray[4]
            tidx1 = temp3.indexOf( " " ); // 这里假定无仅仅'\t'分隔
            tidx2 = temp3.indexOf( "," );
            if( tidx1<0 && tidx2<0 ){
                continue;
            }
            if( tidx1>=0 ){
                tidx = tidx1;
                if( tidx2>=0 && tidx2<tidx1 ) tidx = tidx2;
            }
            else
                tidx = tidx2;
            tidx++;
            while( temp3.charAt(tidx) == ' ' || temp3.charAt(tidx) == '\t' ) tidx++;// 过滤掉首端空格
            var temp4 = temp3.substr( tidx );
            farray[4] = parseFloat(temp4);
    
    //取数值farray[5]
            tidx1 = temp4.indexOf( " " ); // 这里假定无仅仅'\t'分隔
            tidx2 = temp4.indexOf( "," );
            if( tidx1<0 && tidx2<0 ){
                continue;
            }
            if( tidx1>=0 ){
                tidx = tidx1;
                if( tidx2>=0 && tidx2<tidx1 ) tidx = tidx2;
            }
            else
                tidx = tidx2;
            tidx++;
            while( temp4.charAt(tidx) == ' ' || temp4.charAt(tidx) == '\t' ) tidx++;// 过滤掉首端空格
            var temp5 = temp4.substr( tidx );
            farray[5] = parseFloat(temp5);
            
            //alert( "parser matrix="+farray[0]+" "+farray[1]+" "+farray[2]+" "+farray[3]+" "+farray[4]+" "+farray[5]+" " );
            var mmm = new Array(6);
            mmm[0] = tempm[0]*farray[0]+tempm[2]*farray[1];
            mmm[1] = tempm[1]*farray[0]+tempm[3]*farray[1];
            mmm[2] = tempm[0]*farray[2]+tempm[2]*farray[3];
            mmm[3] = tempm[1]*farray[2]+tempm[3]*farray[3];
            mmm[4] = tempm[0]*farray[4]+tempm[2]*farray[5]+tempm[4];
            mmm[5] = tempm[1]*farray[4]+tempm[3]*farray[5]+tempm[5];
            
            for( var i=0; i<6; i++ ) tempm[i] = mmm[i];
        }//end while

        var ttt = new Array(6);
        ttt[0] = tempm[0]*MV[0]+tempm[2]*MV[1];
        ttt[1] = tempm[1]*MV[0]+tempm[3]*MV[1];
        ttt[2] = tempm[0]*MV[2]+tempm[2]*MV[3];
        ttt[3] = tempm[1]*MV[2]+tempm[3]*MV[3];
        ttt[4] = tempm[0]*MV[4]+tempm[2]*MV[5]+tempm[4];
        ttt[5] = tempm[1]*MV[4]+tempm[3]*MV[5]+tempm[5];
        
        for( var i=0; i<6; i++ ) MV[i] = ttt[i];

        parent = parent.getParentNode();
    }

}
/*
function fileOpen()
{
	OnFileOpen();
}*/
var idArray = new Array("100A50025I1111041","100A50025I1111042","100A50025I1111043",
			"100A50025I4111041","100A50025I4111042","100A50025I4111043",
			"100A50025I5111041","100A50025I5111042","100A50025I5111043",
			"100B50022F0111041","100B50022F0211041","100B50022F0311041",
			"100B50022F0411041","100B50022H0111041","100B50022H0211041",
			"100B50022I0111041","100B50022I0211041","100B50022I0311041",
			"100B50022I0411041","100B50022I0511041","100B50022I0611041",
			"100B50022I0711041","100B50022I0811041","100B50022I0911041",
			"100B50029H0111041","100B50029H0211041","100B50029N0111041",
			"100B50029N0211041","100B50029N0311041","100B50029N0411041",
			"100B50029N0511041","100B50029N0611041","100A50025I1111111",
			"100A50025I1111112","100A50025I4111111","100A50025I4111112",
			"100A50025I5111111","100A50025I5111112","100B50022F0111111",
			"100B50022F0111112","100B50022F0211111","100B50022F0211112",
			"100B50022F0311111","100B50022F0311112","100B50022F0411111",
			"100B50022F0411112","100B50022H0111111","100B50022H0111112",
			"100B50022H0111113","100B50022H0211111","100B50022H0211112",
			"100B50022H0211113","100B50022I0111111","100B50022I0111112",
			"100B50022I0111113","100B50022I0211111","100B50022I0211112",
			"100B50022I0211113","100B50022I0311111","100B50022I0311112",
			"100B50022I0311113","100B50022I0411111","100B50022I0411112",
			"100B50022I0411113","100B50022I0511111","100B50022I0511112",
			"100B50022I0511113","100B50022I0611111","100B50022I0611112",
			"100B50022I0611113","100B50022I0711111","100B50022I0711112",
			"100B50022I0711113","100B50022I0811111","100B50022I0811112",
			"100B50022I0811113","100B50022I0911111","100B50022I0911112",
			"100B50022I0911113","100B50029H0111111","100B50029H0211111");

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
var docroot=null;
var TrueCoords = null;
var GrabPoint = null;

var whole_graph_scale = 1;
var whole_scale =1;

var changemyCustomMenu = false;

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
h=getInnerHeight()-30;

//w = document.body.clientWidth;
//h = document.body.clientHeight;

try{
// debug2 = svgdok.getElementById("debug2");
docroot=document.rootElement;

TrueCoords = docroot.createSVGPoint();
GrabPoint = docroot.createSVGPoint();
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
svgdoc.setAttribute("height","100%");
svgdoc.setAttribute("width","100%");


//alert(w+","+h);
var ddbox = svgdoc.getAttribute("viewBox");
var ss_box_array;
ss_box_array = ddbox.split(" ");
var scale = 1;
var scale1 = w/ss_box_array[2];
var scale2 = h/ss_box_array[3];
if(scale1 < scale2)
	scale = scale1;
else
	scale = scale2;
	
var headlayer = svgdoc.getElementById("Head_Layer");
		
		//alert(headlayer.getChildNodes().length);
try{
if(headlayer.getChildNodes().length>3 && clientProcessor.currentFileName.indexOf("间隔")<=0)
	{ 
	var s1 = scale1;
	var s2 = 0;
	var s3 = 0;
	var s4 = scale2;
	//var s5 = (-ss_box_array[0]*scale)+(w-ss_box_array[0]*scale)/2;
	var s5 = -ss_box_array[0]*scale1+(w-(scale1*ss_box_array[2]))/2;
	var s6 = -ss_box_array[1]*scale2+(h-(scale2*ss_box_array[3]))/2;
	}
	else{
	var s1 = scale;
	var s2 = 0;
	var s3 = 0;
	var s4 = scale;
	//var s5 = (-ss_box_array[0]*scale)+(w-ss_box_array[0]*scale)/2;
	var s5 = -ss_box_array[0]*scale+(w-(scale*ss_box_array[2]))/2;
	var s6 = -ss_box_array[1]*scale+(h-(scale*ss_box_array[3]))/2;
 } 
}catch(ex)
{ 
	var s1 = scale;
	var s2 = 0;
	var s3 = 0;
	var s4 = scale;
	//var s5 = (-ss_box_array[0]*scale)+(w-ss_box_array[0]*scale)/2;
	var s5 = -ss_box_array[0]*scale+(w-(scale*ss_box_array[2]))/2;
	var s6 = -ss_box_array[1]*scale+(h-(scale*ss_box_array[3]))/2;
}

whole_scale = scale;

//var s5 = -ss_box_array[0]*scale;
//alert("matrix("+s1+","+s2+","+s3+","+s4+","+s5+","+s6+")");
svgmap.setAttribute("transform","matrix("+s1+","+s2+","+s3+","+s4+","+s5+","+s6+")");

svgdoc.removeAttribute("viewBox");

svgdoc.setAttribute("viewBox1",ddbox);
svgdoc.setAttribute("scale1",scale);

}catch(e){
}

// var gesamt = 0;
// for(var i = 'A'.charCodeAt(0); i <= 'Z'.charCodeAt(0); i++) {
// laenge = texttest.getComputedTextLength() / 100;
// gesamt += laenge;
// texttest = svgdok.getElementById('text-A');
// texttest.firstChild.data = 'gemessen ' + (texttest.getComputedTextLength()) + ' gerechnet ' + gesamt;
}
function syncMenu(evt) {

if(evt==null){return;}

if(navigation==null || navigation=="" || zoompoint==null || zoompoint=="" || zoomtext==null || zoomtext=="" ||
   svgmap==null || svgmap=="" || grundstuecke==null || grundstuecke=="" || ttrelem==null || ttrelem=="" ||
   tttelem==null || tttelem=="" || tooltip==null || tooltip=="")
{
	svgdok=evt.getTarget().getOwnerDocument();
	zoompoint=svgdok.getElementById("zoompoint");
	zoomtext=svgdok.getElementById("zoomtext");
	svgmap=svgdok.getElementById("whole_graph");
	navigation=svgdok.getElementById("navigation");
	grundstuecke=svgdok.getElementById("grundstuecke");
	ttrelem=svgdok.getElementById("ttr");
	tttelem=svgdok.getElementById("ttt");
	tooltip=svgdok.getElementById("tooltip");
}

var s = 1/docroot.currentScale;
var ct = docroot.currentTranslate;
var tf="matrix("+s+" 0 0 "+s+" "+(-ct.x)*s+" "+(-ct.y)*s+")";
try{
navigation.setAttribute( "transform", tf );
}catch(ex){}
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
/*if (minY < minLoadedY) { ladeSektoren(); } else if (minX < minLoadedX) { ladeSektoren(); } else if (maxY > maxLoadedY) { ladeSektoren(); } else if (maxX > maxLoadedX) { ladeSektoren(); }*/
}
function zoomMapStart(evt,zoom) {
if(navigation==null || navigation=="" || zoompoint==null || zoompoint=="" || zoomtext==null || zoomtext=="" ||
   svgmap==null || svgmap=="" || grundstuecke==null || grundstuecke=="" || ttrelem==null || ttrelem=="" ||
   tttelem==null || tttelem=="" || tooltip==null || tooltip=="")
{
	svgdok=evt.getTarget().getOwnerDocument();
	zoompoint=svgdok.getElementById("zoompoint");
	zoomtext=svgdok.getElementById("zoomtext");
	svgmap=svgdok.getElementById("whole_graph");
	navigation=svgdok.getElementById("navigation");
	grundstuecke=svgdok.getElementById("grundstuecke");
	ttrelem=svgdok.getElementById("ttr");
	tttelem=svgdok.getElementById("ttt");
	tooltip=svgdok.getElementById("tooltip");
}
	
var specialFactor = docroot.currentScale / 2 * (1 - zoom);
docroot.currentScale = docroot.currentScale * zoom;
docroot.currentTranslate.y = docroot.currentTranslate.y + (getInnerHeight() * specialFactor);
docroot.currentTranslate.x = docroot.currentTranslate.x + (getInnerWidth() * specialFactor);
if( MapZoomTimeout )
clearTimeout( MapZoomTimeout );
MapZoomTimeout = setTimeout( "zoomMap(" + zoom + ")", 1000/docroot.currentScale );
}
function zoomMap(zoom) {
if(evt==null) {alert("zoom map halt");return;}

if(navigation==null || navigation=="" || zoompoint==null || zoompoint=="" || zoomtext==null || zoomtext=="" ||
   svgmap==null || svgmap=="" || grundstuecke==null || grundstuecke=="" || ttrelem==null || ttrelem=="" ||
   tttelem==null || tttelem=="" || tooltip==null || tooltip=="")
{
	svgdok=evt.getTarget().getOwnerDocument();
	zoompoint=svgdok.getElementById("zoompoint");
	zoomtext=svgdok.getElementById("zoomtext");
	svgmap=svgdok.getElementById("whole_graph");
	navigation=svgdok.getElementById("navigation");
	grundstuecke=svgdok.getElementById("grundstuecke");
	ttrelem=svgdok.getElementById("ttr");
	tttelem=svgdok.getElementById("ttt");
	tooltip=svgdok.getElementById("tooltip");
}


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

function Grab(evt)
{
	 	/*if(evt.button==2 && !changemyCustomMenu){
		  try{
		  //clientProcessor.composeStationMenuList();
		  
		  var newMenuRoot = parseXML( printNode( document.getElementById("myCustomMenu")), contextMenu ); 
		  contextMenu.replaceChild( newMenuRoot.firstChild, contextMenu.firstChild ); 
		  changemyCustomMenu = true;
		  }catch(ex){}
	 	}*/
	
         GetTrueCoords(evt);
         GrabPoint.x = TrueCoords.x ;
         GrabPoint.y = TrueCoords.y ;
};


function Drag(evt)
{
//         GetTrueCoords(evt);
};


function Drop(evt)
{
    GetTrueCoords(evt);
    docroot.currentTranslate.y = docroot.currentTranslate.y+Math.round(TrueCoords.y-GrabPoint.y);
	docroot.currentTranslate.x = docroot.currentTranslate.x+Math.round(TrueCoords.x-GrabPoint.x);
};

function GetTrueCoords(evt)
{
     var newScale = docroot.currentScale;
     var translation = docroot.currentTranslate;
     TrueCoords.x = (evt.clientX - translation.x)/newScale;
     TrueCoords.y = (evt.clientY - translation.y)/newScale;
};

function fullScreenMap(){
try{
	var embedobj = oo("svgcanvas");
	var __svgdoc = embedobj.getSVGDocument();
	docroot=__svgdoc.rootElement;

	docroot.currentScale = 1 ; 
	docroot.currentTranslate.y = 0;
	docroot.currentTranslate.x = 0;
	
	}
catch(ex){
	return ;
	}
}

function ShowTooltip(evt,text) {

if(navigation==null || navigation=="" || zoompoint==null || zoompoint=="" || zoomtext==null || zoomtext=="" ||
   svgmap==null || svgmap=="" || grundstuecke==null || grundstuecke=="" || ttrelem==null || ttrelem=="" ||
   tttelem==null || tttelem=="" || tooltip==null || tooltip=="")
{
	svgdok=evt.getTarget().getOwnerDocument();
	zoompoint=svgdok.getElementById("zoompoint");
	zoomtext=svgdok.getElementById("zoomtext");
	svgmap=svgdok.getElementById("whole_graph");
	navigation=svgdok.getElementById("navigation");
	grundstuecke=svgdok.getElementById("grundstuecke");
	ttrelem=svgdok.getElementById("ttr");
	tttelem=svgdok.getElementById("ttt");
	tooltip=svgdok.getElementById("tooltip");
}

if (hover) {
var strz=evt.getTarget();
var pos=strz.getAttribute("id").indexOf("_");
if (pos==-1) {
//strz.parentNode.setAttribute("stroke","black");
//strz.parentNode.setAttribute("stroke-width",eval(strz.parentNode.parentNode.getStyle().getPropertyValue("stroke-width")) + 2);
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

if(navigation==null || navigation=="" || zoompoint==null || zoompoint=="" || zoomtext==null || zoomtext=="" ||
   svgmap==null || svgmap=="" || grundstuecke==null || grundstuecke=="" || ttrelem==null || ttrelem=="" ||
   tttelem==null || tttelem=="" || tooltip==null || tooltip=="")
{
	svgdok=evt.getTarget().getOwnerDocument();
	zoompoint=svgdok.getElementById("zoompoint");
	zoomtext=svgdok.getElementById("zoomtext");
	svgmap=svgdok.getElementById("whole_graph");
	navigation=svgdok.getElementById("navigation");
	grundstuecke=svgdok.getElementById("grundstuecke");
	ttrelem=svgdok.getElementById("ttr");
	tttelem=svgdok.getElementById("ttt");
	tooltip=svgdok.getElementById("tooltip");
}

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

var svgns   = "http://www.w3.org/2000/svg";
var xlinkns = "http://www.w3.org/1999/xlink";

function makeShape(evt) {
    if ( window.svgDocument == null )
        svgDocument = evt.target.ownerDocument;

    var svgRoot = svgDocument.documentElement;

    var defs = svgDocument.createElementNS(svgns, "defs");
    
    var rect = svgDocument.createElementNS(svgns, "rect");
    rect.setAttributeNS(null, "id", "rect");
    rect.setAttributeNS(null, "width", 15);
    rect.setAttributeNS(null, "height", 15);
    rect.setAttributeNS(null, "style", "fill: green");

    defs.appendChild(rect);
    
    var tagslayer = svgDocument.createElementNS(svgns, "g");
    tagslayer.setAttributeNS(null, "id", "tagsLayer");
    tagslayer.setAttributeNS(null, "style", "fill: green");
    
    var gtag = svgDocument.createElementNS(svgns, "g");
    gtag.setAttributeNS(null, "id", "33333333335");
    gtag.setAttributeNS(null, "style", "fill: green");
    
    var use1 = svgDocument.createElementNS(svgns, "use");
    use1.setAttributeNS(null, "x", 5);
    use1.setAttributeNS(null, "y", 5);
    use1.setAttributeNS(xlinkns, "xlink:href", "#rect");
    use1.setAttributeNS(xlinkns, "class", "BKBV-110KV");
    
    use2 = svgDocument.createElementNS(svgns, "use");
    use2.setAttributeNS(null, "x", 30);
    use2.setAttributeNS(null, "y", 30);
    use2.setAttributeNS(xlinkns, "xlink:href", "#Tag:shape3");
    use2.setAttributeNS(xlinkns, "class", "BKBV-110KV");
    use2.setAttributeNS(xlinkns, "stroke", "rgb(255,255,0)");
    
    gtag.appendChild(use1);
    gtag.appendChild(use2);
    tagslayer.appendChild(gtag);
    
    svgRoot.appendChild(defs);
    svgRoot.appendChild(tagslayer);
}
