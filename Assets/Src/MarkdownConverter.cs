using System;
using System.IO;
using UnityEngine;

namespace FUI {
    public class MarkdownConverter {

        public Color UrlColor = Color.green;
        public Color SeparatorColor = Color.gray;
        public Color CodeBackgroundColor = new Color(0.25f, 0.25f, 0.25f);

        int _indent = 0;

        public MarkdownConverter() {
            var form = Form.Current;
            var theme = form?.Theme ?? Theme.Default;
            UrlColor = theme.PrimaryColor;
            //SeparatorColor = SeparatorColor;
            //CodeBackgroundColor = CodeBackgroundColor;
        }


        public string Convert(string markdownText) {
            /*var settings = new CommonMark.CommonMarkSettings {
                AdditionalFeatures =
            };*/
            var markdownDocument = CommonMark.CommonMarkConverter.Parse(markdownText);
            _indent = 0;
            var unityText = MarkdownToUnityRichText(markdownDocument);
            return unityText;
        }

        private string MarkdownToUnityRichText(CommonMark.Syntax.Block document) {
            // Use a StringWriter to generate Unity rich text
            using (var writer = new StringWriter()) {
                ConvertBlockToUnityRichText(document, writer);
                return writer.ToString();
            }
        }

        private void ConvertBlockToUnityRichText(CommonMark.Syntax.Block block, StringWriter writer) {

            void IncIndent() {
                _indent++;
                writer.Write($"<indent={_indent}em>");
            }
            void DecIndent() {
                _indent--;
                writer.Write($"<indent={_indent}em>");
            }
            void VerticalSpace(float size) {
                writer.Write($"<size={size}em>\n</size>");
            }
            void Separator() {
                var color = ColorUtility.ToHtmlStringRGBA(SeparatorColor);
                writer.Write($"<size=1><align=flush><color=#{color}><mark=#{color}>__</mark></color></align></size>");
            }

            while (block != null) {
                switch (block.Tag) {
                    case CommonMark.Syntax.BlockTag.Document:
                    ConvertBlockToUnityRichText(block.FirstChild, writer);
                    break;

                    case CommonMark.Syntax.BlockTag.Paragraph:
                    writer.Write("<color=white>");
                    ConvertInlineToUnityRichText(block.InlineContent, writer);
                    writer.WriteLine("</color>");
                    VerticalSpace(0.5f);
                    break;

                    case CommonMark.Syntax.BlockTag.AtxHeading: { 
                        var headerSize = Math.Pow(1.15f, 7 - block.Heading.Level); // Scale font size for headers
                        writer.Write($"<size={headerSize}em><b>");
                        VerticalSpace(0.5f);

                        ConvertInlineToUnityRichText(block.InlineContent, writer);
                        writer.WriteLine();

                        VerticalSpace(0.5f);
                        writer.Write("</b></size>");
                    }
                    break;

                    case CommonMark.Syntax.BlockTag.SetextHeading: {
                        var headerSize = Math.Pow(1.15f, 7 - block.Heading.Level); // Scale font size for headers
                        writer.Write($"<size={headerSize}em><b>");
                        VerticalSpace(0.5f);
                        ConvertInlineToUnityRichText(block.InlineContent, writer);
                        writer.WriteLine();
                        VerticalSpace(0.2f);
                        Separator();
                        VerticalSpace(0.3f);
                        writer.WriteLine("</b></size>");
                    }
                    break;



                    case CommonMark.Syntax.BlockTag.List:
                    IncIndent();
                    ConvertBlockToUnityRichText(block.FirstChild, writer);
                    DecIndent();
                    break;

                    case CommonMark.Syntax.BlockTag.ListItem:
                        if (block.ListData.ListType==CommonMark.Syntax.ListType.Bullet)
                            writer.Write("•");
                        else
                            writer.Write($"{block.ListData.Start}.");
                    IncIndent();
                    ConvertBlockToUnityRichText(block.FirstChild, writer);
                    DecIndent();
                    break;


                    default:
                    Debug.LogWarning($"not implemented block tag {block.Tag.ToString()}");
                    break;
                    // Add more cases for other block types like BlockQuote, Code, etc.
                }

                block = block.NextSibling;
            }
        }

        private void ConvertInlineToUnityRichText(CommonMark.Syntax.Inline inline, StringWriter writer) {
            while (inline != null) {
                switch (inline.Tag) {
                    case CommonMark.Syntax.InlineTag.String:
                    writer.Write(inline.LiteralContent);
                    break;

                    case CommonMark.Syntax.InlineTag.Emphasis:
                    writer.Write("<i>");
                    ConvertInlineToUnityRichText(inline.FirstChild, writer);
                    writer.Write("</i>");
                    break;

                    case CommonMark.Syntax.InlineTag.Strong:
                    writer.Write("<b>");
                    ConvertInlineToUnityRichText(inline.FirstChild, writer);
                    writer.Write("</b>");
                    break;

                    case CommonMark.Syntax.InlineTag.Link:
                    writer.Write($"<color=#{ColorUtility.ToHtmlStringRGBA(UrlColor)}><u><link=\"{inline.TargetUrl}\">");
                    ConvertInlineToUnityRichText(inline.FirstChild, writer);
                    writer.Write("</link></u></color>");
                    break;


                    case CommonMark.Syntax.InlineTag.Code: {
                        var color = ColorUtility.ToHtmlStringRGBA(CodeBackgroundColor);
                        writer.Write($"<mark=#{color}> {inline.LiteralContent} <size=0>.</size></mark>");
                    }
                    break;


                    default:
                    Debug.LogWarning($"not implemented inline tag {inline.Tag}");
                    break;
                }

                inline = inline.NextSibling;
            }
        }



    }
}
