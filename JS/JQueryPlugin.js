// jQuery 插件开发
$.fn.myFirstPlugin =function(options){
	
	//默认参数值
	var defaults ={
		'color':'red',
		'fontSize':'12px'
	};
	//合并参数。没有传递参数的使用默认参数，有的即覆盖默认参数
	var settings = $.extend({},defaults,options);
	
	//this 指的是用jQuery选中的元素
	this.css({
		'color':settings.color,
		'fontSize':settings.fontSize
		});
	return this.each(function() {
		//对每个元素进行操作
		$(this).append(' ' + $(this).attr('href'));
	})
}






//使插件面向对象，更好维护  参考https://www.cnblogs.com/ajianbeyourself/p/5815689.html
;(function($, window,document,undefined) { //使用自调用匿名函数包裹代码，防止污染全局变量
    //构造函数
    var Jeckfirst = function(ele, opt) {
        this.$element = ele,
            this.defaults = {
                'color': 'red',
                'fontSize': '12px',
                'textDecoration': 'none'
            },
            this.options = $.extend({}, this.defaults, opt)
    }
    // 定义方法
    Jeckfirst.prototype = {
        zhou: function() {
            return this.$element.css({
                'color': this.options.color,
                'fontSize': this.options.fontSize,
                'textDecoration': this.options.textDecoration
            })
        }
    }
    //在插件中使用Jeckfirst对象
    $.fn.myFirstPlugin = function(options) {
        //创建Jeckfirst的实体
        var first = new Jeckfirst(this, options);
        //调用其方法
        return first.zhou();
    }
})(jQuery,window,document);



/* example 
<ul>
    <li>
        <a href="http://www.webo.com/liuwayong">我的微博</a>
    </li>
    <li>
        <a href="http://http://www.cnblogs.com/Wayou/">我的博客</a>
    </li>
    <li>
        <a href="http://wayouliu.duapp.com/">我的小站</a>
    </li>
</ul>
<p>这是p标签不是a标签，我不会受影响</p>
<script src="jquery-1.11.0.min.js"></script>
<script src="jquery.myplugin.js"></script>
<script type="text/javascript">
    $(function(){
        $('a').myFirstPlugin();
    })
</script>*/