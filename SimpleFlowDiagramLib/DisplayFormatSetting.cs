#region .NET
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

namespace SimpleFlowDiagramLib
{
    public class DisplayFormatSetting
    {
        #region protected properties
        protected string _SettingKey = null;
        protected string _ForeColorName = "Black";
        protected string _NegativeForeColor = "Red";
        protected string _BackColorName = "White";
        protected string _NegativeBackColor = "White";

        protected string _FontFamilyName = "Arial";
        protected float _FontSize = 8;
        protected string _FontStyleString = "Regular";
        protected string _FormatString = null;
        #endregion

        #region public accessors
        public string SettingKey
        {
            get { return _SettingKey; }
            set { _SettingKey = value; }
        }

        #region Colors
        public string ForeColorName
        {
            get { return _ForeColorName; }
            set { _ForeColorName = value; }
        }

        public Color ForeColor
        {
            get
            {
                Color cr = Color.Black;
                cr = Color.FromName(ForeColorName);
                return cr;
            }
        }

        public string NegativeForeColorName
        {
            get { return _NegativeForeColor; }
            set { _NegativeForeColor = value; }
        }

        public Color NegativeForeColor
        {
            get
            {
                Color cr = Color.Black;
                cr = Color.FromName(NegativeForeColorName);
                return cr;
            }
        }

        public string BackColorName
        {
            get { return _BackColorName; }
            set { _BackColorName = value; }
        }

        public Color BackColor
        {
            get
            {
                Color cr = Color.Black;
                cr = Color.FromName(BackColorName);
                return cr;
            }
        }

        public string NegativeBackColorName
        {
            get { return _NegativeBackColor; }
            set { _NegativeBackColor = value; }
        }

        public Color NegativeBackColor
        {
            get
            {
                Color cr = Color.Black;
                cr = Color.FromName(NegativeBackColorName);
                return cr;
            }
        }
        #endregion

        #region Font related
        public string FontFamilyName
        {
            get { return _FontFamilyName; }
            set { _FontFamilyName = value; }
        }

        public FontFamily SelectedFontFamily
        {
            get
            {
                if (!string.IsNullOrEmpty(_FontFamilyName))
                {
                    return new FontFamily(_FontFamilyName);
                }
                else
                {
                    return new FontFamily("Arial");
                }
            }
        }

        public float FontSize
        {
            get { return _FontSize; }
            set { _FontSize = value; }
        }

        public string FontStyleString
        {
            get { return _FontStyleString; }
            set { _FontStyleString = value; }
        }

        public FontStyle SelectedFontStyle
        {
            get
            {
                FontStyle Result;
                FontStyle sty;
                string[] Tokens = null;
                int i = 0;

                Result = FontStyle.Regular;

                if (!string.IsNullOrEmpty(FontStyleString))
                {
                    Tokens = FontStyleString.Split('|');
                    if (Tokens != null && Tokens.Length > 0)
                    {
                        i = 0;
                        foreach (string Token in Tokens)
                        {
                            if (i == 0)
                            {
                                if (Enum.TryParse(Token, true, out sty))
                                {
                                    Result = sty;
                                }
                            }
                            else
                            {
                                if (Enum.TryParse(Token, true, out sty))
                                    if (Result != sty)
                                    {
                                        Result = Result | sty;
                                    }
                            }
                        }

                        i++;
                    }
                }
                else
                {
                    Result = FontStyle.Regular;
                }

                return Result;
            }
        }

        public Font SelectedFont
        {
            get
            {
                Font fnt = new Font(SelectedFontFamily, FontSize, SelectedFontStyle);

                return fnt;
            }
        }
        #endregion

        public string FormatString
        {
            get { return _FormatString; }
            set { _FormatString = value; }
        }
        #endregion

        public DisplayFormatSetting Clone()
        {
            DisplayFormatSetting Copy = new DisplayFormatSetting();
            Copy.SettingKey = this.SettingKey;
            Copy.ForeColorName = this.ForeColorName;
            Copy.NegativeForeColorName = this.NegativeForeColorName;
            Copy.BackColorName = this.BackColorName;
            Copy.NegativeBackColorName = this.NegativeBackColorName;
            Copy.FontFamilyName = this.FontFamilyName;
            Copy.FontSize = this.FontSize;
            Copy.FontStyleString = this.FontStyleString;
            Copy.FormatString = this.FormatString;
            return Copy;
        }
    }
}
