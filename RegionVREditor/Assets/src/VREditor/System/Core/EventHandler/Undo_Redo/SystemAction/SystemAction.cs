using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class SystemAction
{


  
    

    public event EventHandler DO;
    public event EventHandler REDO;
    public event EventHandler UNDO;



    public void Redo()
    {
        if (REDO != null)
        {
            REDO(this,null);
        }
    }

    public void Undo()
    {
        if (UNDO != null)
        {
            UNDO(this, null);
        }
    }
}