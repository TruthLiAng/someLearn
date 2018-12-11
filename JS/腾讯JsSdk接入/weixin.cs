 public string GetToken()
        {
            JsTokenModal token = new JsTokenModal();
            token = (JsTokenModal)Session["CurrentToken"];
            if (token == null || token.GetTime.AddSeconds(7000) < DateTime.Now)
            {
                var tokenUrl = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + AppId + "&secret=" + AppSecret;
                var new_token = JsonConvert.DeserializeObject<WeiXinTokenModal>(SendGetRequest(tokenUrl));

                token = new JsTokenModal();
                token.AccessToken = new_token.access_token;
                token.GetTime = DateTime.Now;

                //用户授权模型保存在Session中
                Session["CurrentToken"] = token;
                Session.Timeout = 4000;
            }
            return token.AccessToken;
        }

        public string GetSignature(string url)
        {
            var token = GetToken();
            var ticketUrl = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token=" + token + "&type=jsapi";
            var ticketModal = JsonConvert.DeserializeObject<WeiXinTicketModal>(SendGetRequest(ticketUrl));

            var ticket = ticketModal.ticket;

            Dictionary<string, string> dicPara = new Dictionary<string, string>();

            dicPara.Add("noncestr", "123qweasdzxc");
            dicPara.Add("jsapi_ticket", ticket);
            dicPara.Add("timestamp", "1414587457");
            dicPara.Add("url", url);

            var vDic = (from objDic in dicPara orderby objDic.Key ascending select objDic);

            StringBuilder str = new StringBuilder();
            foreach (KeyValuePair<string, string> kv in vDic)
            {
                string pkey = kv.Key;
                string pvalue = kv.Value;
                str.Append(pkey + "=" + pvalue + "&");
            }

            string result = str.ToString().Substring(0, str.ToString().Length - 1);

            var sha = System.Security.Cryptography.SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(result));

            var sb = new StringBuilder();
            foreach (var t in sha)
            {
                sb.Append(t.ToString("X2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 发起一个GET类型的Web请求，并返回响应
        /// </summary>
        /// <param name="requestUri">请求地址</param>
        /// <returns>响应</returns>
        public static string SendGetRequest(string requestUri)
        {
            if (requestUri.ToLower().Trim().StartsWith("https"))
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            }
            //创建GET请求
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUri);
            httpWebRequest.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
            string content = reader.ReadToEnd();
            //注意请求完成需要关闭流和上下文
            reader.Close();
            responseStream.Close();
            response.Close();
            return content;
        }