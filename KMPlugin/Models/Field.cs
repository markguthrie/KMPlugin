namespace KMPlugin.Models
{
    internal class Field
    {
        public string ID { get; set; }
        public string Value { get; set; }

        public Field(string iD="", string value = "")
        {
            ID = iD;
            Value = value;
        }
    }
}
