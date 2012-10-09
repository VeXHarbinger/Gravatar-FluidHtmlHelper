using System;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;


    /// <summary>
    /// Gravatar Fluid Html Helper Builder
    /// </summary>
    public static class GravatarFluidHtmlHelper
    {
        /// <summary>
        /// Gravatars fluid HTML helper.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <returns></returns>
        public static GravatarImage GravatarImage(this HtmlHelper htmlHelper)
        {
            return new GravatarImage(htmlHelper);
        }
    }

    /// <summary>
    /// Gravatar Fluid Html Helper
    /// </summary>
    public class GravatarImage : IHtmlString
    {
        /// <summary>
        /// Internal placeholder for the HTML Tag builder
        /// </summary>
        private TagBuilder builder;

        /// <summary>
        /// Internal placeholder for the Default Image value
        /// </summary>
        private DefaultImageOptions defaultImage;

        /// <summary>
        /// Internal placeholder for the defaultImageUrl value
        /// </summary>
        private string defaultImageUrl;

        /// <summary>
        /// Internal placeholder for the email Address value
        /// </summary>
        private string emailAddress;

        /// <summary>
        /// Internal placeholder for the force Default Image value
        /// </summary>
        private bool forceDefaultImage;

        /// <summary>
        /// Internal placeholder for the force Secure Request value
        /// </summary>
        private bool forceSecureRequest;

        /// <summary>
        /// Internal placeholder for the HTML helper Tag builder
        /// </summary>
        private HtmlHelper helper;

        /// <summary>
        /// Is the current view context secure?
        /// </summary>
        private bool isSecureConnection;

        /// <summary>
        /// Internal placeholder for the Rating value
        /// </summary>
        private RatingOptions rating;

        /// <summary>
        /// Internal placeholder for the size value
        /// </summary>
        private int size;

        /// <summary>
        /// Initializes a new instance of the <see cref="GravatarImage"/> class.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        public GravatarImage(HtmlHelper htmlHelper)
        {
            isSecureConnection = htmlHelper.ViewContext.HttpContext.Request.IsSecureConnection;

            this.helper = htmlHelper;
            this.builder = new TagBuilder("img");
        }

        /// <summary>
        /// In addition to allowing you to use your own image, Gravatar has a number of built in options which you can also use as defaults. Most of these work by taking the requested email hash and using it to generate a themed image that is unique to that email address
        /// </summary>
        public enum DefaultImageOptions
        {
            /// <summary>Default Gravatar logo</summary>
            [DescriptionAttribute("")]
            Default,

            /// <summary>404 - do not load any image if none is associated with the email hash, instead return an HTTP 404 (File Not Found) response</summary>
            [DescriptionAttribute("404")]
            Http404,

            /// <summary>Mystery-Man - a simple, cartoon-style silhouetted outline of a person (does not vary by email hash)</summary>
            [DescriptionAttribute("mm")]
            MysteryMan,

            /// <summary>Identicon - a geometric pattern based on an email hash</summary>
            [DescriptionAttribute("identicon")]
            Identicon,

            /// <summary>MonsterId - a generated 'monster' with different colors, faces, etc</summary>
            [DescriptionAttribute("monsterid")]
            MonsterId,

            /// <summary>Wavatar - generated faces with differing features and backgrounds</summary>
            [DescriptionAttribute("wavatar")]
            Wavatar,

            /// <summary>Retro - awesome generated, 8-bit arcade-style pixelated faces</summary>
            [DescriptionAttribute("retro")]
            Retro
        }

        /// <summary>
        /// A list of possible gravatar default option for not found images
        /// </summary>
        public enum GravatarDefaultUrlOptions
        {
            /// <summary>
            /// No default URL option is provided
            /// </summary>
            None,

            /// <summary>
            /// Return the Identicon image if gravatar image not found
            /// </summary>
            Identicon,

            /// <summary>
            /// Return the Monsterid image if gravatar image not found
            /// </summary>
            Monsterid,

            /// <summary>
            /// Return the Wavatar image if gravatar image not found
            /// </summary>
            Wavatar,

            /// <summary>
            /// Return a 404 HTTP error if gravatar image not found
            /// </summary>
            Error,

            /// <summary>
            /// Return the specified image (CustomDefault) if gravatar image not found
            /// </summary>
            Custom
        }

        /// <summary>
        /// A list of possible Gravatar image rating
        /// </summary>
        /// <remarks>
        /// Gravatar allows users to self-rate their images so that they can indicate if an image is appropriate for a certain audience. By default, only 'G' rated images are displayed unless you indicate that you would like to see higher ratings
        /// </remarks>
        public enum RatingOptions
        {
            /// <summary>Suitable for display on all websites with any audience type</summary>
            [DescriptionAttribute("g")]
            G = 0,

            /// <summary>May contain rude gestures, provocatively dressed individuals, the lesser swear words, or mild violence</summary>
            [DescriptionAttribute("pg")]
            PG = 1,

            /// <summary>May contain such things as harsh profanity, intense violence, nudity, or hard drug use</summary>
            [DescriptionAttribute("r")]
            R = 2,

            /// <summary>May contain hardcore sexual imagery or extremely disturbing violence</summary>
            [DescriptionAttribute("x")]
            X = 3
        }

        /// <summary>
        /// Returns an HTML-encoded string.
        /// </summary>
        /// <returns>
        /// An HTML-encoded string.
        /// </returns>
        string IHtmlString.ToHtmlString()
        {
            if (String.IsNullOrEmpty(emailAddress))
            {
                throw new Exception("emailAddress is required");
            }
            builder.Attributes.Add("src",
                string.Format("{0}://{1}.gravatar.com/avatar/{2}?s={3}{4}{5}{6}",
                   processHTTPConnection(),
                    processSecureRequest(),
                    processEmailAddress(),
                    size.ToString(),
                    processDefaultImage(),
                    forceDefaultImage ? "&f=y" : "",
                    "&r=" + rating.ToString()
                    )
                );

            builder.Attributes.Add("class", "gravatar");
            builder.Attributes.Add("alt", "Gravatar image");
            return new HtmlString(builder.ToString(TagRenderMode.SelfClosing)).ToString();
        }

        /// <summary>
        /// A URL-encoded address for a custom image
        /// </summary>
        /// <remarks>
        /// If you'd prefer to use your own default image (perhaps your logo, a funny face, whatever), then you can easily do so by supplying the URL to an image in the d= or default= parameter.
        /// </remarks>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="defaultImage">The default image.</param>
        /// <returns></returns>
        public GravatarImage DefaultImage(DefaultImageOptions defaultImage)
        {
            this.defaultImage = defaultImage;
            return this;
        }

        /// <summary>
        /// The url to a custom image to use inplace of the default one.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="defaultImageUrl">The default image URL.</param>
        /// <returns></returns>
        public GravatarImage DefaultImageUrl(string defaultImageUrl)
        {
            this.defaultImageUrl = defaultImageUrl;

            return this;
        }

        /// <summary>
        /// The Users Email Address.
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <returns></returns>
        public GravatarImage EmailAddress(string emailAddress)
        {
            this.emailAddress = emailAddress;
            return this;
        }

        public GravatarImage EncodeDefaultImageUrl(bool ShouldEncode = true)
        {
            if (ShouldEncode)
            {
                if (string.IsNullOrEmpty(defaultImageUrl))
                {
                    throw new Exception("You must set the Default Image Url before calling encode");
                }
                else
                {
                    this.defaultImageUrl = this.helper.Encode(this.defaultImageUrl);
                }
            }

            return this;
        }

        public GravatarImage EncodeDefaultImageUrl(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                this.defaultImageUrl = this.helper.Encode(url);
            }
            return this;
        }

        /// <summary>
        /// Forces the default image to load.
        /// </summary>
        /// <param name="forceDefaultImage">if set to <c>true</c> [force default image].</param>
        /// <returns></returns>
        public GravatarImage ForceDefaultImage(bool forceDefaultImage)
        {
            this.forceDefaultImage = forceDefaultImage;
            return this;
        }

        /// <summary>
        /// If you need to Force secure request.
        /// </summary>
        /// <remarks>
        /// If you're displaying Gravatars on a page that is being served over SSL (e.g. the page URL starts with HTTPS), then you'll want to serve your Gravatars via SSL as well, otherwise you'll get annoying security warnings in most browsers.
        /// </remarks>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="forceSecureRequest">if set to <c>true</c> [force secure request].</param>
        /// <returns></returns>
        public GravatarImage ForceSecureRequest(bool forceSecureRequest)
        {
            this.forceSecureRequest = forceSecureRequest;
            return this;
        }

        /// <summary>
        /// The appropriate rating for the site's general audience.
        /// </summary>
        /// <remarks>
        /// Gravatar allows users to self-rate their images so that they can indicate if an image is appropriate for a certain audience. By default, only 'G' rated images are displayed unless you indicate that you would like to see higher ratings. Using the r= or rating= parameters, you may specify one of the following ratings to request images up to and including that rating
        /// </remarks>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="rating">The rating.</param>
        /// <returns></returns>
        public GravatarImage Rating(RatingOptions rating)
        {
            this.rating = rating;
            return this;
        }

        /// <summary>
        /// The size of the image you would like returned.  Default is 80px by 80px if no size parameter is supplied.
        /// </summary>
        /// <remarks>
        /// By default, images are presented at 80px by 80px if no size parameter is supplied. You may request a specific image size, which will be dynamically delivered from Gravatar by using the s= or size= parameter and passing a single pixel dimension (since the images are square):
        /// </remarks>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public GravatarImage Size(int size)
        {
            this.size = size;
            return this;
        }

        /// <summary>
        /// The url to a custom image to use inplace of the default one.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="defaultImageUrl">The default image URL.</param>
        /// <returns></returns>
        public GravatarImage Tooltip(string tooltip)
        {
            builder.Attributes.Add("title", tooltip);
            return this;
        }

        /// <summary>
        /// Processes the default image.
        /// </summary>
        /// <returns></returns>
        internal string processDefaultImage()
        {
            return "&d=" + (!string.IsNullOrEmpty(this.defaultImageUrl) ? this.defaultImageUrl.ToString() : this.defaultImage.ToString());
        }

        /// <summary>
        /// Processes the HTTP connection.
        /// </summary>
        /// <returns></returns>
        internal string processHTTPConnection()
        {
            return isSecureConnection || forceSecureRequest ? "https" : "http";
        }

        /// <summary>
        /// Processes the secure request.
        /// </summary>
        /// <returns></returns>
        internal string processSecureRequest()
        {
            return isSecureConnection || forceSecureRequest ? "secure" : "www";
        }

        /// <summary>
        /// Generates an MD5 hash of the given string
        /// </summary>
        /// <remarks>Source: http://msdn.microsoft.com/en-us/library/system.security.cryptography.md5.aspx </remarks>
        private string processEmailAddress()
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(this.emailAddress.ToString()));

            // Create a new String builder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
}