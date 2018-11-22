function onApiLoaded() {
    var map = new AMap.Map('container', {
        center: [104.065724, 30.657433],
        zoom: 11,
        resizeEnable: true
    });

    //ToolBar
    map.plugin(["AMap.ToolBar"], function () {
        map.addControl(new AMap.ToolBar());
    });
    if (location.href.indexOf('&guide=1') !== -1) {
        map.setStatus({ scrollWheel: false })
    }

    var geocoder = new AMap.Geocoder({
    });

    //输入提示
    var autoOptions = {
        input: "tipinput"
    };
    var auto = new AMap.Autocomplete(autoOptions);
    var placeSearch = new AMap.PlaceSearch({
    });  //构造地点查询类

    AMap.event.addListener(auto, "select", select);//注册监听，当选中某条记录时会触发
    function select(e) {
        placeSearch.setCity(e.poi.adcode);
        placeSearch.setCityLimit(true);
        placeSearch.search(e.poi.name, searchBack);  //关键字查询查询
    };

    function searchBack(status, result) {
        // 查询成功时，result即对应匹配的POI信息

        var pois = result.poiList.pois;
        for (var i = 0; i < pois.length; i++) {
            var poi = pois[i];
            var marker = [];
            marker[i] = new AMap.Marker({
                //map: map,
                position: poi.location,   // 经纬度对象，也可以是经纬度构成的一维数组[116.39, 39.9]
                title: poi.name
            });

            //绑定鼠标点击事件——弹出信息创口1

            marker[i].on('click', function (res) {
                openInfo(res);
            });

            map.add(marker[i]);
        }
        map.setFitView();
    }

    //在指定位置打开信息窗体
    function openInfo(e) {
        geocoder.getAddress(e.lnglat, function (status, result) {
            if (status === 'complete' && result.regeocode) {
                var address = result.regeocode.formattedAddress;
                var adcode = "";
                if (result.regeocode.addressComponent.adcode) {
                    adcode = result.regeocode.addressComponent.adcode;
                }

                //构建信息窗体中显示的内容
                var info = [];
                info.push('<input type="hidden" id="lnglat" value="' + e.lnglat.lng + ',' + e.lnglat.lat + '" />');
                info.push('<input type="hidden" id="address" value="' + address + '" />');
                info.push("<p class='input-item'>邮编 : " + adcode + "</p>");
                info.push("<p class='input-item'>地址 :" + address + "</p>");
                info.push('<input type="button" class="layui-btn layui-btn-primary layui-btn-xs" value="选择" onclick="setMapDataToActivity()"/></div></div>')
                infoWindow = new AMap.InfoWindow({
                    content: info.join("")  //使用默认信息窗体框样式，显示信息内容
                });
                infoWindow.open(map, e.lnglat);
            } else { alert(JSON.stringify(result)) }
        });
    }
}

var url = 'https://webapi.amap.com/maps?v=1.4.10&key=31028764320360c16550431627af7cd5&plugin=AMap.Autocomplete,AMap.PlaceSearch,AMap.MouseTool,AMap.Geocoder&callback=onApiLoaded';
var jsapi = document.createElement('script');
jsapi.charset = 'utf-8';
jsapi.src = url;
document.head.appendChild(jsapi);

function setMapDataToActivity() {
    var index = parent.layer.getFrameIndex(window.name); //获取窗口索引

    var lnglat = document.getElementById('lnglat').value;
    var lng = lnglat.split(',')[0];
    var lat = lnglat.split(',')[1];
    var address = document.getElementById('address').value;

    var parentIFrame = parent.$('#layui-layer-iframe1')[0];

    parentIFrame.ownerDocument.getElementById('activityLng').value = lng;
    parentIFrame.ownerDocument.getElementById('activityLat').value = lat;
    parentIFrame.ownerDocument.getElementById('activityPlace').value = address;

    parent.layer.close(index);
}