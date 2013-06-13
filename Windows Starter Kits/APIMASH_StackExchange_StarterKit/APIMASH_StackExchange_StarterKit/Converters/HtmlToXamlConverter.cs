using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace APIMASH_StackExchange_StarterKit.Converters
{
    public class ContainerEvtArgs
    {
        public ContainerEvtArgs()
        { }

        public string SpanId { get; set; }
        public InlineUIContainer Container { get; set; }
    }

    public delegate void ContainerDisplayHandler(ContainerEvtArgs e);

    public static class HtmlToXamlConverter
    {
        static StringBuilder sbXaml = null;

        // The following are used to track the number of items to be replaced.
        private static Int32 BqCount = 0;
        private static Int32 DlCount = 0;
        private static Int32 OlCount = 0;
        private static Int32 UlCount = 0;
        private static Int32 TbCount = 0;

        private static Boolean IsFirstRun = true;

        //event for replacing inline containers
        public static event ContainerDisplayHandler ContainerCreated;

        public static string ConvertHtmlToXaml(string content)
        {
            PrepareSecondRun();
            IsFirstRun = true;

            sbXaml = new StringBuilder(/*"<Span xml:space=\"preserve\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">"*/);

            HandleHtmlContent(content);
            return sbXaml.ToString();
        }

        #region HTML Parsing

        /// <summary>
        /// handles all html content
        /// </summary>
        /// <param name="content"></param>
        private static void HandleHtmlContent(string content)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(content);
            doc.OptionCheckSyntax = true;
            doc.OptionFixNestedTags = true;
            doc.OptionAutoCloseOnEnd = true;
            //doc.OptionOutputAsXml = true;
            //doc.OptionDefaultStreamEncoding = Encoding.UTF8;

            if (doc.DocumentNode.HasChildNodes)
            {
                HandleChildren(doc.DocumentNode.ChildNodes);
                PrepareSecondRun();
                HandleChildren(doc.DocumentNode.ChildNodes);
            }
        }

        /// <summary>
        /// prepare for the replacement run
        /// </summary>
        private static void PrepareSecondRun()
        {
            //set the FirstRun flag to false as it is completed
            IsFirstRun = false;

            BqCount = 0;
            DlCount = 0;
            OlCount = 0;
            UlCount = 0;
            TbCount = 0;
        }

        /// <summary>
        /// Handles child nodes for a given node
        /// </summary>
        /// <param name="nodes"></param>
        private static void HandleChildren(HtmlNodeCollection nodes)
        {
            foreach (HtmlNode itm in nodes)
            {
                if (itm.Name.ToLower().Equals("html"))
                {
                    if (itm.Element("body") != null)
                        HandleChildren(itm.Element("body").ChildNodes);
                }
                else
                    if (IsFirstRun)
                        HandleHtmlNode(itm);
                    else
                        HandleHtmlNodeReplacements(itm);
            }
        }

        /// <summary>
        /// handles individual html tags
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="content"></param>
        /// <param name="attr"></param>
        private static void HandleHtmlNode(HtmlNode htmNode)
        {
            switch (htmNode.Name.ToLower())
            {
                case "html":
                case "body":
                    HandleChildren(htmNode.ChildNodes);
                    break;
                case "div":
                    sbXaml.Append(DivXaml(htmNode));
                    break;

                case "br":
                    if (ValidateParagraphWrap(htmNode))
                        sbXaml.Append("<LineBreak />");
                    else
                        sbXaml.Append("<Paragraph />");
                    break;
                case "pre":
                    sbXaml.Append(BlockQuoteXaml(htmNode));
                    break;
                case "code":
                    sbXaml.Append(BlockQuoteXaml(htmNode));
                    break;
                case "blockquote":
                    sbXaml.Append(BlockQuoteXaml(htmNode));
                    break;

                case "a":
                    //xaml must be within a paragraph tag this ensures the tags are wrapped correctly
                    if (ValidateParagraphWrap(htmNode))
                        sbXaml.Append(HyperlinkXaml(htmNode.InnerText, htmNode.Attributes["href"].Value));
                    else
                        sbXaml.Append("<Paragraph>" + HyperlinkXaml(htmNode.InnerText, htmNode.Attributes["href"].Value) + "</Paragraph>");
                    break;

                case "i":
                    sbXaml.Append(RunXaml(htmNode.InnerText, false, true));
                    break;

                case "p":
                    sbXaml.Append(ParagraphXaml(htmNode.InnerText, false, true));
                    break;

                case "dl":
                    sbXaml.Append(DefinitionListXaml(htmNode));
                    DlCount++;
                    break;

                case "ol":
                    sbXaml.Append(OrderedListXaml(htmNode));
                    OlCount++;
                    break;

                case "ul":
                    sbXaml.Append(UnorderedListXaml(htmNode));
                    UlCount++;
                    break;

                case "b":
                case "strong":
                    if (ValidateParagraphWrap(htmNode))
                        sbXaml.Append("<Paragraph>" + RunXaml(htmNode.InnerText, true, false) + "</Paragraph>");
                    else
                        sbXaml.Append(RunXaml(htmNode.InnerText, true, false));
                    break;
                case "h1":
                case "h2":
                case "h3":
                case "h4":
                case "h5":
                case "h6":
                case "h7":
                    sbXaml.Append(HeaderXaml(htmNode.InnerText, htmNode.Name));
                    break;
                case "table":
                    sbXaml.Append(HtmlTableXaml(htmNode));
                    TbCount++;
                    break;
                //case "sub":
                //    sbXaml.Append(RunSubSupXaml(content, 1));
                //    break;
                //case "sup":
                //    sbXaml.Append(RunSubSupXaml(content, 2));
                //    break;
                //default:
                //    if (!ValidateParagraphWrap(htmNode))
                //        sbXaml.Append("<Paragraph>" + RunXaml(htmNode.InnerText, false, false) + "</Paragraph>");
                //    else
                //        sbXaml.Append(RunXaml(htmNode.InnerText, false, false));
                //    break;
            }
        }

        /// <summary>
        /// handles individual html tags
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="content"></param>
        /// <param name="attr"></param>
        private static void HandleHtmlNodeReplacements(HtmlNode htmNode)
        {
            switch (htmNode.Name.ToLower())
            {
                case "blockquote":
                    sbXaml.Append(BlockQuoteXaml(htmNode));
                    BqCount++;
                    break;

                case "dl":
                    sbXaml.Append(DefinitionListXaml(htmNode));
                    DlCount++;
                    break;

                case "ol":
                    sbXaml.Append(OrderedListXaml(htmNode));
                    OlCount++;
                    break;

                case "ul":
                    sbXaml.Append(UnorderedListXaml(htmNode));
                    UlCount++;
                    break;

                case "table":
                    sbXaml.Append(HtmlTableXaml(htmNode));
                    TbCount++;
                    break;
            }
        }

        private static Boolean ValidateParagraphWrap(HtmlNode htmNode)
        {
            return ((htmNode.PreviousSibling != null && htmNode.PreviousSibling.Name.ToLower().Equals("p"))
                    || htmNode.ParentNode.Name.ToLower().Equals("p"));
        }

        #endregion

        #region Xaml Builders


        /// <summary>
        /// Div Handler
        /// </summary>
        /// <param name="cntnt"></param>
        /// <returns></returns>
        private static string DivXaml(HtmlNode cntnt)
        {
            bool hasContent = false;
            StringBuilder sbUL = new StringBuilder("<Paragraph><Span>");
            foreach (HtmlNode itm in cntnt.ChildNodes)
            {
                if (!itm.Name.ToLower().Equals("script") && !String.IsNullOrWhiteSpace(itm.InnerText.Replace("\r", "").Replace("\n", "")))
                {
                    hasContent = true;
                    HandleHtmlNode(itm);
                }
            }
            sbUL.Append("</Span></Paragraph>");

            if (hasContent)
                return sbUL.ToString();
            else
                return string.Empty;
        }

        /// <summary>
        /// creates a paragraph
        /// </summary>
        /// <param name="cntnt"></param>
        /// <param name="isBold"></param>
        /// <param name="isItalic"></param>
        /// <returns></returns>
        private static string ParagraphXaml(string cntnt, bool isBold = false, bool isItalic = false)
        {
            return "<Paragraph><Run " + (isBold ? "FontWeight=\"Bold\"" : "") + (isItalic ? "FontStyle=\"Italic\"" : "") + " Text=\"" + CleanContent(cntnt) + "\" /></Paragraph>";
        }

        /// <summary>
        /// creates a run
        /// </summary>
        /// <param name="cntnt"></param>
        /// <param name="isBold"></param>
        /// <param name="isItalic"></param>
        /// <returns></returns>
        private static string RunXaml(string cntnt, bool isBold = false, bool isItalic = false)
        {
            return "<Run " + (isBold ? "FontWeight=\"Bold\"" : "") + (isItalic ? "FontStyle=\"Italic\"" : "") + " Text=\"" + CleanContent(cntnt) + "\" />";
        }

        private static string HeaderXaml(string cntnt, string header)
        {
            int baseFontSize = 20;
            int hdrSize = Int32.Parse(header.Substring(1, 1)) * 2;

            return "<Paragraph><Run FontWeight=\"Bold\" FontSize=\"" + (baseFontSize - hdrSize).ToString() + "\" Text=\"" + CleanContent(cntnt) + "\" /></Paragraph>";
        }


        /// <summary>
        /// handles sub/super Script
        /// </summary>
        /// <param name="cntnt"></param>
        /// <param name="itm"></param>
        /// <returns></returns>
        private static string RunSubSupXaml(string cntnt, int itm)
        {
            if (itm.Equals(1))
                return "<Run FontSize=\"10\" Text=\"" + CleanContent(cntnt) + "\" />";
            else
                return "<Run FontSize=\"10\" Text=\"" + CleanContent(cntnt) + "\" />";
        }

        /// <summary>
        /// creates a hyperlink form an expected tag...<a href="1ch+27:17">27:17</A>
        /// </summary>
        /// <param name="cntnt"></param>
        /// <returns></returns>
        private static string HyperlinkXaml(string cntnt, string href)
        {
            return ("<Hyperlink TextDecorations=\"Underline\" NavigateUri=\"" + href + "\">" + cntnt + "</Hyperlink>");
        }


        /// <summary>
        /// blockqoute handler
        /// </summary>
        /// <param name="cntnt"></param>
        /// <returns></returns>
        private static string BlockQuoteXaml(HtmlNode cntnt)
        {
            if (IsFirstRun)
            {
                return ("<Paragraph TextAlignment=\"Left\"><Span FontFamily=\"rp_bq" + BqCount.ToString() + "\" /></Paragraph>");
            }
            else
            {
                InlineUIContainer ilContainer = new InlineUIContainer();

                TextBlock tbItm = new TextBlock();
                tbItm.Margin = new Thickness(25, 0, 0, 0);
                tbItm.TextWrapping = TextWrapping.Wrap;
                tbItm.Text = cntnt.InnerText;

                ilContainer.Child = tbItm;

                OnContainerCreated("rp_bq" + BqCount.ToString(), ilContainer);

                return string.Empty;
            }
        }


        /// <summary>
        /// Definitions list builder
        /// </summary>
        /// <param name="cntnt"></param>
        /// <returns></returns>
        private static string DefinitionListXaml(HtmlNode cntnt)
        {
            if (IsFirstRun)
            {
                return ("<Paragraph TextAlignment=\"Left\"><Span FontFamily=\"rp_dl" + DlCount.ToString() + "\" /></Paragraph>");
            }
            else
            {
                InlineUIContainer ilContainer = new InlineUIContainer();

                StackPanel spUL = new StackPanel();
                //set the stapanel margin               

                //create a textblock for displaying the text
                System.Collections.Generic.IList<String> contentList = new System.Collections.Generic.List<String>();

                TextBlock tbItm = null;
                foreach (HtmlNode itm in cntnt.ChildNodes)
                {
                    tbItm = new TextBlock();
                    switch (itm.Name.ToLower())
                    {
                        case "dt":
                            tbItm.Text = itm.InnerText;
                            tbItm.Margin = new Thickness(15, 0, 0, 0);
                            spUL.Children.Add(tbItm);
                            break;

                        case "dd":
                            tbItm.Text = itm.InnerText;
                            tbItm.Margin = new Thickness(25, 0, 0, 0);
                            spUL.Children.Add(tbItm);
                            break;
                    }
                }
                tbItm = null;
                ilContainer.Child = spUL;

                //fire the event
                OnContainerCreated("rp_dl" + DlCount.ToString(), ilContainer);

                //add the container to the control
                //HandlXamlReplacements("rp_dl" + DlCount.ToString(), ilContainer);

                return String.Empty;
            }
        }

        /// <summary>
        /// Definitions list builder
        /// </summary>
        /// <param name="cntnt"></param>
        /// <returns></returns>
        private static string OrderedListXaml(HtmlNode cntnt)
        {
            if (IsFirstRun)
            {
                return ("<Paragraph TextAlignment=\"Left\"><Span FontFamily=\"rp_ol" + OlCount.ToString() + "\" /></Paragraph>");
            }
            else
            {
                InlineUIContainer ilContainer = new InlineUIContainer();
                StackPanel spUL = new StackPanel();

                //set the stapanel margin
                spUL.Margin = new Thickness(25, 0, 0, 0);

                //create a textblock for displaying the text
                System.Collections.Generic.IList<String> contentList = new System.Collections.Generic.List<String>();
                int ictr = 1;
                TextBlock tbItm = null;
                foreach (HtmlNode itm in cntnt.ChildNodes)
                {
                    if (itm.Name.Equals("li"))
                    {
                        tbItm = new TextBlock();
                        tbItm.Text = ictr.ToString() + ".  " + itm.InnerText;
                        spUL.Children.Add(tbItm);
                        ictr++;
                    }
                }
                ilContainer.Child = spUL;

                //fire the event
                OnContainerCreated("rp_ol" + OlCount.ToString(), ilContainer);

                //add the container to the control
                //HandlXamlReplacements("rp_ol" + OlCount.ToString(), ilContainer);

                return String.Empty;
            }
        }

        private static string UnorderedListXaml(HtmlNode cntnt)
        {
            if (IsFirstRun)
            {
                return ("<Paragraph TextAlignment=\"Left\"><Span FontFamily=\"rp_ul" + UlCount.ToString() + "\" /></Paragraph>");
            }
            else
            {
                InlineUIContainer ilContainer = new InlineUIContainer();
                StackPanel spUL = new StackPanel();

                //set the stapanel margin
                spUL.Margin = new Thickness(25, 0, 0, 0);

                //create a textblock for displaying the text
                System.Collections.Generic.IList<String> contentList = new System.Collections.Generic.List<String>();
                TextBlock tbItm = null;
                foreach (HtmlNode itm in cntnt.ChildNodes)
                {
                    if (itm.Name.Equals("li"))
                    {
                        tbItm = new TextBlock();
                        tbItm.Text = "« " + itm.InnerText;
                        spUL.Children.Add(tbItm);
                    }
                }
                ilContainer.Child = spUL;

                //fire the event
                OnContainerCreated("rp_ul" + UlCount.ToString(), ilContainer);

                //add the container to the control
                //HandlXamlReplacements("rp_ul" + UlCount.ToString(), ilContainer);

                return string.Empty;
            }
        }

        private static string HtmlTableXaml(HtmlNode cntnt)
        {
            if (IsFirstRun)
            {
                return ("<Paragraph TextAlignment=\"Left\"><Span FontFamily=\"rp_tb" + TbCount.ToString() + "\" /></Paragraph>");
            }
            else
            {
                Grid tableGrid = null;
                //var grdWidth = CalulateItemWidth(cntnt, RTB.ActualWidth);

                IList<HtmlNode> rows =
                   (from tblNode in cntnt.ChildNodes
                    where (tblNode.Name.ToLower().Equals("tr"))
                    select tblNode).ToList();

                //CREATE Grid rows and cols
                var trNode = rows[0];
                int colCount =
                      (from tdNode in trNode.ChildNodes
                       where (tdNode.Name.ToLower().Equals("td"))
                       select tdNode).Count<HtmlNode>();

                if (rows.Count > 0 && colCount > 0)
                {
                    tableGrid = new Grid();
                    //if (grdWidth > 0)
                    //    tableGrid.Width = grdWidth;
                    //set up rows
                    for (int ictr = 0; ictr < rows.Count; ictr++)
                    {
                        tableGrid.RowDefinitions.Add(new RowDefinition());
                    }

                    //setup cols
                    for (int jctr = 0; jctr < colCount; jctr++)
                    {
                        Double wdth = CalulateItemWidth(trNode.ChildNodes[jctr], tableGrid.Width);

                        if (wdth > 0)
                            tableGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(wdth) });
                        else
                            tableGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    }
                }


                if (rows.Count > 0 && colCount > 0)
                {
                    for (int trCtr = 0; trCtr < rows.Count; trCtr++)
                    {
                        IList<HtmlNode> cols = (from tdNode in rows[trCtr].ChildNodes
                                                where (tdNode.Name.ToLower().Equals("td"))
                                                select tdNode).ToList();
                        for (int tdCtr = 0; tdCtr < cols.Count; tdCtr++)
                        {
                            var tbcnt = new TextBlock {Text = cols[tdCtr].InnerText};

                            tbcnt.SetValue(Grid.RowProperty, trCtr);
                            tbcnt.SetValue(Grid.ColumnProperty, tdCtr);

                            tableGrid.Children.Add(tbcnt);
                        }
                        cols = null;
                    }
                }


                var ilContainer = new InlineUIContainer {Child = tableGrid};

                //fire the event
                OnContainerCreated("rp_tb" + TbCount.ToString(), ilContainer);

                return string.Empty;
            }
        }

        private static Double CalulateItemWidth(HtmlNode hn, double parentWidth)
        {
            double itemWidth = 0;
            if (hn.HasAttributes && hn.Attributes["width"] != null)
            {
                if (hn.Attributes["width"].Value.EndsWith("%") && parentWidth > 0)
                {
                    Double.TryParse(hn.Attributes["width"].Value.Replace("%", string.Empty), out itemWidth);
                    itemWidth = (itemWidth / 100) * parentWidth; //calulate with based on actual with of the rtb
                }
                else
                    Double.TryParse(hn.Attributes["width"].Value, out itemWidth);
            }
            return itemWidth;
        }
        #endregion

        #region  utility methods

        private static string CleanContent(string content)
        {
            var sbc = new StringBuilder(content);
            sbc.Replace("&", "&amp;");
            sbc.Replace("\"", "&quot;");
            sbc.Replace("'", "`");
            sbc.Replace("\n'", "");
            sbc.Replace("\r", "");
            return sbc.ToString();
        }

        /// <summary>
        /// this method fires the container created event.
        /// </summary>
        /// <param name="spanId"></param>
        /// <param name="ilContainer"></param>
        private static void OnContainerCreated(string spanId, InlineUIContainer ilContainer)
        {
            if ( ContainerCreated != null )
                ContainerCreated(new ContainerEvtArgs {SpanId = spanId, Container = ilContainer});
        }


        #endregion

        public new static string ToString()
        {
            sbXaml.Replace("\n", "");
            sbXaml.Replace("\r", "");
            // sbXaml.Replace("<Paragraph><Span></Span></Paragraph>", "");
            return sbXaml.ToString() /*+ "</Span>"*/;
        }

    }
}
