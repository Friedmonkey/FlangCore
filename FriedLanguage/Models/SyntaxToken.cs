using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.Models
{
    public struct SyntaxToken
    {
        public SyntaxType Type { get; set; }
        public int Position { get; set; }
        public int EndPosition => Position + Text.Length;
        public object Value { get; set; }
        public string Text { get; set; }

        public SyntaxToken(SyntaxType type, int pos, object val, string txt)
        {
            Type = type;
            Position = pos;
            Value = val;
            Text = txt;
        }
        public SyntaxToken(SyntaxType type,object val,string text = "")
        {
            Type = type;
            Value = val;
            Text = text;
        }
        public SyntaxToken(SyntaxToken Source)
        {
            Type = Source.Type;
            Position = Source.Position;
            Value = Source.Value;
            Text = Source.Text;
        }
        public override string ToString()
        {
            return Type.ToString().PadRight(16) + " at " + Position.ToString().PadRight(3) + " with val: " + (Value ?? "null").ToString().PadRight(16) + " text: " + Text.ToString().PadRight(16);
        }

        public override bool Equals(object obj)
        {
            if (obj is SyntaxToken syn)
            {
                return (this.Type == syn.Type && this.Position == syn.Position && this.Value == syn.Value && this.Text == syn.Text);
            }
            return false;
        }
    }
}
