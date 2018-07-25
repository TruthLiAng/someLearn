    let post = ({url,data,successHandler,errorHandler})=>{
      //1. 实例化XMLHttpRequest对象
      let xhr = new XMLHttpRequest();
      //2. 打开请求
      xhr.open('post',url);
      //3. 设定返回值类型为json
      xhr.responseType = "json";
      //4. 设置头部信息
      xhr.setRequestHeader('Accept','application/json');
      //* 当前数据编码为表单编码
      //xhr.setRequestHeader('Content-Type','application/x-www-form-urlencoded')
      //* 当前数据编码为json
      xhr.setRequestHeader('Content-Type','application/json')
      xhr.onreadystatechange = function(){
          //console.log(this.readystate);
          if(this.readyState == 4){
              if(this.status == 200){
                  // success
                  successHandler(this.response);
              } else {
                  //
                  errorHandler(this.response);
              }
          }
      };
      //5. 直接请求
      //将数据转换为查询字符串
      xhr.send(data);
      //xhr.send(urlEncoded(data));
      //将数据转换为json
      xhr.send(JSON.stringify(data));
  }