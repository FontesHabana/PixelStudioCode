using PixelWallE.Language.Parsing.Expressions;
using System.Collections.Generic;

namespace PixelWallE.Language.Parsing;

 public class Scope
    {   //Permite extender el lenguaje a contextos globales y bloques de codigo
        public readonly Dictionary<string,(object, ExpressionType)> variables=new();
        public readonly Dictionary<string,Label> labels=new();
        //
        private readonly Scope? parent;
        //Se pueden agregar las funciones aquí;
        public Scope(  List<Label> labels,Scope? parent=null){
            
            this.parent=parent;
            foreach (var item in labels)
            {
                this.labels.Add(item.LabelReference,item);
            }
        }

        public bool DeclareVariable(string name, object value, ExpressionType type){
            if (variables.ContainsKey(name))
            {
                return false;
            }
                variables[name]=(value,type);
                return true;
        }
    
        public bool AssignVariable(string name, object value, ExpressionType type){
            if (variables.ContainsKey(name))
            {   variables[name]=(value,type);
                return true;
            }else if(parent!=null){
                return parent.AssignVariable(name, value, type);
            }
           
            return false;
        }

        public object? GetVariable(string name){
            if (variables.ContainsKey(name)) 
            {
                return variables[name].Item1;
            }
            return parent?.GetVariable(name);
        }
        
        public ExpressionType GetVariableType(string name){
            if (variables.ContainsKey(name)) 
            {
                return variables[name].Item2;
            }
            if (parent==null)
            {
                return ExpressionType.Anytype;
            }
            return parent.GetVariableType(name);
        }
        public bool IsDeclaredVariable(string name){
            if (variables.ContainsKey(name)) 
                return true;
            else if(parent!=null)
            {
                return parent.IsDeclaredVariable(name);
            }
            return false;
        }
                
        
         public bool IsDeclaredLabel(string name){
            if (labels.ContainsKey(name)) 
                return true;
             else if(parent!=null)
            {
                return parent.IsDeclaredLabel(name);
            }
            return false;
        }
                
        public Label? GetLabel(string name){
            if (labels.ContainsKey(name)) 
            {
                return labels[name];
            }
            return parent?.GetLabel(name);
        }
        
         //Aqui hay DRY revisa
                
        }