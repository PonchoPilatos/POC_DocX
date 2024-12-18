using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_prueba
{
    internal class DocAnswerDto
    {
        private string value { get; set; }
        private string aux_text1 { get; set; }
        private string aux_text2 { get; set; }
        private string? font { get; set; }
        private int? size { get; set; }
        private bool? bold_type { get; set; }
        private bool? italics { get; set; }
        private string? position{ get; set; }
        private string? total_position { get; set; }

        public DocAnswerDto(string value, string aux_text1, string aux_text2, string position) {
            this.value = value;
            this.aux_text1 = aux_text1;
            this.aux_text2 = aux_text2;
            this.position = position;
            font = null;
            size = null;
            bold_type = null;
            italics = null;
            total_position = null;
        }

        public DocAnswerDto(string value, string aux_text1, string aux_text2, string font, int size, bool bold_type, bool italics, string total_position)
        {
            this.value = value;
            this.aux_text1 = aux_text1;
            this.aux_text2 = aux_text2;
            this.font = font;
            this.size = size;
            this.bold_type = bold_type; 
            this.italics = italics;
            this.total_position = total_position;
            position = null;
        }

        public string Value() { 
            return this.value;
        }

        public string Aux_text1()
        {
            return this.aux_text1;
        }

        public string Aux_text2()
        {
            return this.aux_text2;
        }

        public string Font()
        {
            return this.font;
        }

        public int Size()
        {
            return (int)this.size;
        }

        public bool Bold_type()
        {
            return (bool)this.bold_type;
        }

        public bool Italics()
        {
            return (bool)this.italics;
        }

        public string Position()
        {
            return this.position;
        }

        public string Total_position()
        {
            return this.total_position;
        }
    }
}
