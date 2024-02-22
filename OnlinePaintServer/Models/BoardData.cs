using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlinePaintServer.Models
{
    public class BoardData
    {
        public string boardId { get; set; }
        public string boardName {  get; set; }
        public List<LineObject> lines { get; set; }
        public BoardData(string boardId, string boardName)
        {
            lines = new List<LineObject>();
            this.boardId = boardId;
            this.boardName = boardName;

        }
        public void AppendLines(List<LineObject> newLines)
        {
            lines.AddRange(newLines);
        }

        public void AppendLine(LineObject newLine)
        {
            lines.Add(newLine);
        }
        public override string ToString()
        {
            return boardId+"+"+ boardName;
        }
    }
}
