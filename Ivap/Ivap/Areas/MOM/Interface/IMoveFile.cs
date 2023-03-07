using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivap.Areas.MOM.Interface
{
    interface IMoveFile
    {
        bool MoveFile(int MOMID, string fileName);
    }
}
