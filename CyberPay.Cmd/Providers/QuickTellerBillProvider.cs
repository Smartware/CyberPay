using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CyberPay.Cmd.Payload.Quickteller;
using Newtonsoft.Json;

namespace CyberPay.Cmd.Providers
{
    public class AccessToken
    {
        public String access_token { get; set; }
    }
    public class QuickTellerBillProvider : IQuickTellerBillProvider
    {
        private const String ACCESS_TOKEN = "ACCESS_TOKEN";
        private const String TIMESTAMP = "Timestamp";
        private const String TERMINAL_ID = "TerminalID";
        private const String NONCE = "Nonce";
        private const String SIGNATURE_METHOD = "SignatureMethod";
        private const String SIGNATURE = "Signature";
        private const String AUTHORIZATION = "Authorization";
        private const String AUTHORIZATION_REALM = "InterswitchAuth";
        private const String ISO_8859_1 = "ISO-8859-1";
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public QuickTellerBillProvider()
        {
        }

        public NameEnquiry ValidateName(string bankCode, string accountId)
        {
            var resourceUrl = ConfigurationManager.AppSettings["QuicktellerNameEnquiryUrl"];
            String billresponse = ValidateAccountNumber(resourceUrl, accountId, bankCode);
            var responseobject = JsonConvert.DeserializeObject<NameEnquiry>(billresponse);
            return responseobject;
        }



        public List<QuicktellerBillCategory> GetBillCategories()
        {
            String billresponse = billresponse = SendRequest("", $"{ConfigurationManager.AppSettings["QuicktellerUrl"]}/categorys");
            var responseobject = JsonConvert.DeserializeObject<QuicktellerServiceJSONResponse>(billresponse);
            if (responseobject == null)
            {
                responseobject = new QuicktellerServiceJSONResponse();
            }

            var paymentItems = responseobject.categories;

            if (paymentItems == null)
            {
                responseobject.categories = new List<QuicktellerBillCategory>();
            }

            return responseobject.categories;
        }

        public List<QuicktellerBiller> GetBillers()
        {
            String billresponse = this.SendRequest("", $"{ConfigurationManager.AppSettings["QuicktellerUrl"]}/billers", "GET");

            var responseobject = JsonConvert.DeserializeObject<QuicktellerServiceJSONResponse>(billresponse);

            if (responseobject == null)
            {
                responseobject = new QuicktellerServiceJSONResponse();
            }

            var paymentItems = responseobject.Billers;

            if (paymentItems == null)
            {
                responseobject.Billers = new List<QuicktellerBiller>();
            }

            return responseobject.Billers;
        }

        public QuicktellerPaymentItemsViewModel GetBillerById(string billerId)
        {

            String billresponse = this.SendRequest("", $"{ConfigurationManager.AppSettings["QuicktellerUrl"]}/billers/{billerId}/paymentitems", "GET");

            var responseobject = JsonConvert.DeserializeObject<QuicktellerPaymentItemsViewModel>(billresponse);

            if (responseobject == null)
            {
                responseobject = new QuicktellerPaymentItemsViewModel();
                responseobject.ResponseDescription = $"No match for bill id {billerId}";
            }

            var paymentItems = responseobject.PaymentItems;

            if (paymentItems == null)
            {
                responseobject.PaymentItems = new List<QuicktellerPaymentItem>();
            }

            return responseobject;
        }

        public QuicktellerCustomerViewModel ValidateCustomer(string paymentCode, string subscriberId)
        {

#if DEBUG
            //90101 - airtel
            //dstv - 10428
            //subscriberId = "0000000001";
            //paymentCode = "12001";
#endif
            var customers = new List<QuicktellerCustomerViewModel>();
            customers.Add(new QuicktellerCustomerViewModel() { CustomerId = subscriberId, PaymentCode = paymentCode });
            CustomerValidationModel validationreq = new CustomerValidationModel()
            {
                customers = customers
            };

            String billresponse = this.SendRequest(
            JsonConvert.SerializeObject(validationreq), $"{ConfigurationManager.AppSettings["QuicktellerUrl"]}/customers/validations", "POST");

            var responseobject = JsonConvert.DeserializeObject<QuicktellerServiceJSONResponse>(billresponse);
            var customer = responseobject.customers?.FirstOrDefault();

            if (customer == null)
            {
                customer = new QuicktellerCustomerViewModel()
                {
                    FullName = string.Empty,
                    CustomerId = subscriberId,
                    PaymentCode = paymentCode,
                    ResponseDescription = $"No response received after validating {subscriberId}.",
                    IsSuccessful = false
                };
            }
            else
            {
                customer.FullName = string.IsNullOrEmpty(customer.FullName) ? "Empty" : customer.FullName;
                customer.IsSuccessful = customer?.ResponseCode == "90000" ? true : false;
            }

            return customer;
        }

        public BillsPaymentResponseViewModel SendBillPaymentNotification(string paymentcode,
            string customerUniqueReference, string customerMobile, string customerEmail,
            string transactionUniqueReference, decimal amount)
        {

            BillPaymentNotification paymentNoification = new BillPaymentNotification()
            {
                Amount = (amount * 100).ToString(),
                CustomerEmail = customerEmail,
                CustomerId = customerUniqueReference,
                CustomerMobile = customerMobile,//starts with 234
                PaymentCode = paymentcode,
                RequestReference = ConfigurationManager.AppSettings["QuickTellerPrefix"] + transactionUniqueReference,//max of 8 characters
                TerminalId = ConfigurationManager.AppSettings["TerminalId"]
            };

#if DEBUG
            {
                paymentNoification.CustomerId = "0000000001";
                paymentNoification.PaymentCode = "12001";
            }
#endif
            var convertPayment = JsonConvert.SerializeObject(paymentNoification);
            var billresponse = this.SendRequest(convertPayment,
           $"{ConfigurationManager.AppSettings["QuicktellerUrl"]}/payments/advices", "POST");

            var responseobject = JsonConvert.DeserializeObject<BillsPaymentResponseViewModel>(billresponse);
            return responseobject;
        }



        private string GetRequestToken()
        {
            string responseString = "";
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                ServicePointManager.SecurityProtocol =
                    (SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls) | (SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12);

                String httpMethod = "POST";

                String passportUrl = ConfigurationManager.AppSettings["passportUrl"] ?? "";
                String clientId = ConfigurationManager.AppSettings["clientId"] ?? "";
                String clientSecretKey = ConfigurationManager.AppSettings["clientSecretKey"] ?? "";

                String signatureMethod = "SHA1";//"SHA-1"

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(passportUrl);
                httpWebRequest.Method = httpMethod;
                String clientIdBase64 = GetAsBase64(clientId + ":" + clientSecretKey);
                String authorization = "Basic" + " " + clientIdBase64;


                long timestamp = GetCurrentUnixTimestampMillis() / 1000;
                Guid uuid = Guid.NewGuid();
                String nonce = uuid.ToString().Replace("-", "").Replace("+", "");

                String encodedResourceUrl = UpperCaseUrlEncode(passportUrl);
                String signatureCipher = httpMethod + "&" + encodedResourceUrl + "&"
                  + timestamp + "&" + nonce + "&" + clientId + "&"
                  + clientSecretKey;

                MessageDigest messageDigest = MessageDigest.GetInstance(signatureMethod);
                byte[] signatureBytes = messageDigest
                  .Digest(Encoding.UTF8.GetBytes(signatureCipher));     //    // encode signature as base 64 
                String signature = Convert.ToBase64String(signatureBytes);//.Replace("+","%2B");

                httpWebRequest.Timeout = 60000;
                httpWebRequest.ReadWriteTimeout = 60000;
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                httpWebRequest.KeepAlive = false;


                httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, authorization);
                httpWebRequest.Headers.Add(TIMESTAMP, timestamp.ToString());
                httpWebRequest.Headers.Add(NONCE, nonce);
                httpWebRequest.Headers.Add(SIGNATURE_METHOD, signatureMethod);
                httpWebRequest.Headers.Add(SIGNATURE, signature);

                httpWebRequest.Headers.Add("cache-control", "no-cache");

                String postData = "scope=profile&grant_type=client_credentials"; //JsonConvert.SerializeObject(tokenrequest);
                StreamWriter requestWriter = new StreamWriter(httpWebRequest.GetRequestStream());
                requestWriter.Write(postData);
                requestWriter.Close();

                var response = (HttpWebResponse)httpWebRequest.GetResponse();

                if (HttpStatusCode.OK == response.StatusCode || HttpStatusCode.Created == response.StatusCode)//Successful
                {
                    // Get the stream containing content returned by the server.
                    Stream dataStream = response.GetResponseStream();
                    // Open the stream using a StreamReader for easy access.
                    StreamReader reader = new StreamReader(dataStream);
                    // Read the content.
                    responseString = reader.ReadToEnd();
                    AccessToken retobject = JsonConvert.DeserializeObject<AccessToken>(responseString);
                    responseString = retobject.access_token;

                }
            }
            catch (WebException ex)
            {
                try
                {
                    using (WebResponse response = ex.Response)
                    {
                        HttpWebResponse httpResponse = (HttpWebResponse)response;

                        string text = "";
                        using (Stream data = response.GetResponseStream())
                        {
                            text = new StreamReader(data).ReadToEnd();
                        }
                    }
                }
                catch
                {
                }

            }
            catch (Exception)
            {

            }

            return responseString;
        }

        private string GetAsBase64(string clientId)
        {
            var bytes = Encoding.UTF8.GetBytes(clientId);
            var base64 = Convert.ToBase64String(bytes);
            return base64;
        }

        private string GetAsBase64(byte[] bytes)
        {
            //var bytes = Encoding.UTF8.GetBytes(clientId);
            var base64 = Convert.ToBase64String(bytes);
            return base64;
        }

        public static long GetCurrentUnixTimestampMillis()
        {
            return (long)(DateTime.UtcNow - UnixEpoch).TotalMilliseconds;
        }
        public static string UpperCaseUrlEncode(string s)
        {

            char[] temp = HttpUtility.UrlEncode(s, Encoding.GetEncoding(ISO_8859_1)).ToCharArray();
            for (int i = 0; i < temp.Length - 2; i++)
            {
                if (temp[i] == '%')
                {
                    temp[i + 1] = char.ToUpper(temp[i + 1]);
                    temp[i + 2] = char.ToUpper(temp[i + 2]);
                }
            }
            return new string(temp);
        }

        private string SendRequest(String postData, String resourceUrl, String httpMethod = "GET", String additionalParameters = null)
        {
            string responseString = "";

            try
            {
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                ServicePointManager.SecurityProtocol =
                    (SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls) | (SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12);

                System.Net.ServicePointManager.Expect100Continue = false;

                String clientId = ConfigurationManager.AppSettings["clientId"];
                String clientSecretKey = ConfigurationManager.AppSettings["clientSecretKey"];
                String signatureMethod = "SHA1";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(resourceUrl);
                httpWebRequest.Method = httpMethod;
                String clientIdBase64 = GetAsBase64(clientId);
                String authorization = AUTHORIZATION_REALM + " " + clientIdBase64;
                long timestamp = GetCurrentUnixTimestampMillis() / 1000;
                Guid uuid = Guid.NewGuid();
                String nonce = uuid.ToString("N");//.Replace("-", "").Replace("+", "");

                String encodedResourceUrl = UpperCaseUrlEncode(resourceUrl);
                String signatureCipher = httpMethod + "&" + encodedResourceUrl + "&"
                  + timestamp + "&" + nonce + "&" + clientId + "&"
                  + clientSecretKey;

                if (!String.IsNullOrWhiteSpace(additionalParameters))
                    signatureCipher = signatureCipher + "&" + additionalParameters;

                MessageDigest messageDigest = MessageDigest
                  .GetInstance(signatureMethod);
                byte[] signatureBytes = messageDigest
                  .Digest(Encoding.UTF8.GetBytes(signatureCipher));     //    // encode signature as base 64 
                String signature = Convert.ToBase64String(signatureBytes);//.Replace("+","%2B");

                httpWebRequest.Timeout = 60000;
                httpWebRequest.ReadWriteTimeout = 60000;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.KeepAlive = false;

                String token = GetRequestToken();
                httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "InterswitchAuth " + clientIdBase64);
                httpWebRequest.Headers.Add(TIMESTAMP, timestamp.ToString());
                httpWebRequest.Headers.Add(NONCE, nonce);
                httpWebRequest.Headers.Add(SIGNATURE_METHOD, signatureMethod);
                httpWebRequest.Headers.Add(SIGNATURE, signature);
                httpWebRequest.Headers.Add(ACCESS_TOKEN, token);
                httpWebRequest.Headers.Add(TERMINAL_ID, ConfigurationManager.AppSettings["TerminalId"] ?? "3PBL0001");
                // 3XXT0001

                if (!String.IsNullOrWhiteSpace(postData))
                {
                    StreamWriter requestWriter = new StreamWriter(httpWebRequest.GetRequestStream());
                    requestWriter.Write(postData);
                    requestWriter.Close();
                }

                var response = (HttpWebResponse)httpWebRequest.GetResponse();

                if (HttpStatusCode.Created == response.StatusCode || HttpStatusCode.OK == response.StatusCode)//Successful
                {
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    responseString = reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                try
                {
                    using (WebResponse response = ex.Response)
                    {

                        HttpWebResponse httpResponse = (HttpWebResponse)response;

                        string text = "";
                        using (Stream data = response.GetResponseStream())
                        {
                            text = new StreamReader(data).ReadToEnd();
                        }
                        responseString = text;
                    }
                }
                catch
                {
                }
            }
            catch (Exception)
            {
            }

            return responseString;
        }

        private string ValidateAccountNumber(string remoteUrl, String AccountID, String bankCode)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            #region Properties Definition
            String clientID = ConfigurationManager.AppSettings["EnquiryClientId"];
            String secret = ConfigurationManager.AppSettings["EnquiryClientSecretKey"];
            string terminalID = ConfigurationManager.AppSettings["TerminalId"];

            string httpVerb = "GET";
            String contentType = "application/json"; //charset=utf-8
            string timeStamp = timestamp();

            string percentEncodedUrl = Uri.EscapeDataString(remoteUrl);
            string nonce = GetUniqueKey(20);
            string signatureConValue = httpVerb + "&" + percentEncodedUrl + "&" + timeStamp + "&" + nonce + "&" + clientID + "&" + secret;
            string auth = Base64Encode(clientID);
            string authorization = "InterswitchAuth" + " " + auth;
            string signature = SHA1(signatureConValue);
            #endregion

            String responseString = "";
            #region block of Get Account Validation/Name Enquiry request
            try
            {
                HttpWebRequest webRequestObject = (HttpWebRequest)WebRequest.Create(remoteUrl);
                webRequestObject.ContentType = contentType;
                webRequestObject.Headers.Add("Authorization", authorization);
                webRequestObject.Headers.Add("Signature", signature);
                webRequestObject.Headers.Add("Nonce", nonce);
                webRequestObject.Headers.Add("Timestamp", timeStamp);// timeStamp.ToString());
                webRequestObject.Headers.Add("SignatureMethod", "SHA1");
                webRequestObject.Headers.Add("TerminalID", terminalID);
                webRequestObject.Headers.Add("accountid", AccountID);
                webRequestObject.Headers.Add("bankCode", bankCode);
                webRequestObject.Method = httpVerb;

                HttpWebResponse response = (HttpWebResponse)webRequestObject.GetResponse();
                Stream hResponseStream = response.GetResponseStream();
                StreamReader readStream = new StreamReader(hResponseStream, Encoding.UTF8);
                string RemoteResponse = readStream.ReadToEnd();
                responseString = RemoteResponse.ToString();
                response.Close();
                readStream.Close();

                return responseString;
            }
            catch (WebException wex)
            {
                if (wex.Response != null)
                {
                    using (var errorResponse = (HttpWebResponse)wex.Response)
                    {
                        using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                        {
                            string error = reader.ReadToEnd();
                            //TODO: use JSON.net to parse this string and look at the error message
                            responseString = error.ToString();
                            return responseString;
                        }
                    }
                }
            }
            catch (Exception err)
            {
                throw err;
            }
            finally
            {
            }
            #endregion

            return responseString;
        }


        public static string GetUniqueKey(int maxSize)
        {
            char[] chars = new char[62];
            chars =
            "1234567890".ToCharArray();
            byte[] data = new byte[1];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            data = new byte[maxSize];
            crypto.GetNonZeroBytes(data);
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string SHA1(string plainText)
        {
            SHA1 HashTool = new SHA1Managed();
            Byte[] PhraseAsByte = System.Text.Encoding.UTF8.GetBytes(string.Concat(plainText));
            Byte[] EncryptedBytes = HashTool.ComputeHash(PhraseAsByte);
            HashTool.Clear();
            return Convert.ToBase64String(EncryptedBytes);
        }

        public static string timestamp()
        {
            string timeStamp = ((int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds).ToString();
            return timeStamp.ToString();
        }
    }
}
