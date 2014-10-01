using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using TradingLib.Quant.Base;

namespace TradingLib.Quant.Engine
{
    public abstract class DependencyProcessorBase
    {
        // Fields
        protected List<object> calcOrder;
        protected bool missingObjectsOK;
        public ObjectNameLookup NameLookup;
        protected Dictionary<object, TraversalState> traversalDict = new Dictionary<object, TraversalState>();

        // Methods
        protected DependencyProcessorBase()
        {
        }
        //获得对象的更新次序
        protected IList<object> CalculateObjectUpdateOrder(IEnumerable list)
        {
            this.calcOrder = new List<object>();//更新列表至空
            this.InitTraversalDict(list);
            List<object> stack = new List<object>();
            foreach (object obj2 in new List<object>(this.traversalDict.Keys))
            {
                stack.Add(obj2);
                this.TraverseObject(obj2, stack);
                stack.Clear();
            }
            return this.calcOrder;
        }

        //从对象list中找到依赖obj的对象列表
        protected List<object> GetDependencies(object obj, IEnumerable list, List<object> stack)
        {
            List<object> list2 = new List<object>();
            foreach (object obj2 in list)//遍历list中所有对象
            {
                foreach (object obj3 in this.GetInputs(obj2))//遍历对象的所有的输入
                {
                    if (obj3 != obj) //如果obj!=该输入参数 遍历继续
                    {
                        continue;
                    }
                    if (stack.Contains(obj2)) //如果stack中存在该对象obj2 自己依赖的自己就为循环
                    {
                        string str = "Circular dependency detected: ";
                        foreach (object obj4 in stack)
                        {
                            str = str + this.GetObjectName(obj4) + " ";
                        }
                        throw new Exception();// CircularDependencyException(str + this.GetObjectName(obj2));
                    }
                    if (!list2.Contains(obj2))
                    {
                        list2.Add(obj2);//如果inputs包含obj则将该对象加入list
                    }
                    stack.Add(obj2);//压入stack
                    list2.AddRange(this.GetDependencies(obj2, list, stack));//获得obj2的依赖对象  obj如果依赖obj2 那么obj2的输入也是obj所依赖的
                    stack.RemoveAt(stack.Count - 1);
                    break;
                }
            }
            return list2;
        }
        //获得某个对象的输入参数 需要子类进行具体实现
        protected abstract List<object> GetInputs(object obj);

        //获得某个对象的名称
        public virtual string GetObjectName(object obj)
        {
            if (this.NameLookup != null)
            {
                string str = this.NameLookup(obj);
                if (!string.IsNullOrEmpty(str))
                {
                    return str;
                }
            }
            return obj.GetType().ToString();
        }

        //初始化字典
        protected void InitTraversalDict(IEnumerable list)
        {
            this.traversalDict.Clear();
            foreach (object obj2 in list)
            {
                this.traversalDict[obj2] = TraversalState.NotReached;
            }
        }
        //查看某个对象所依赖的inputs
        protected void TraverseObject(object obj, List<object> stack)
        {
            List<object> inputs = this.GetInputs(obj);//获得该对象的iputs输入对象
            if (!this.traversalDict.ContainsKey(obj))//字典中不存在obj对象
            {
                if ((inputs.Count > 0) && !this.missingObjectsOK)
                {
                    throw new Exception();//RightEdgeError("Input missing while calculating processing order");
                }
            }
            else if (((TraversalState)this.traversalDict[obj]) != TraversalState.Done)//如果该对象的某个输入对象没有完成检查 则需要检查
            {
                if (((TraversalState)this.traversalDict[obj]) == TraversalState.InProgress)//如果该对象在检查过程中,有遇到到需要该对象 则形成了循环依赖
                {
                    string msg = "Circular dependency detected:";
                    foreach (object obj2 in stack)
                    {
                        msg = msg + "\r\n" + this.GetObjectName(obj2);
                    }
                    throw new Exception(); //CircularDependencyException(msg);
                }
                this.traversalDict[obj] = TraversalState.InProgress;//标注该对象正在检查过程中
                foreach (object obj3 in inputs)//循环每个inputs 运算依赖关系
                {
                    stack.Add(obj3);//将inputs加入stack.
                    this.TraverseObject(obj3, stack);//检查该输入的的输入参数
                    stack.RemoveAt(stack.Count - 1);//检查完毕删除该对象
                }
                this.calcOrder.Add(obj);//将该对象加入到计算顺序
                this.traversalDict[obj] = TraversalState.Done;//标记该对象为完成
            }
        }

        // Nested Types
        public delegate string ObjectNameLookup(object obj);

        public enum TraversalState
        {
            NotReached,
            InProgress,
            Done
        }
    }


}
