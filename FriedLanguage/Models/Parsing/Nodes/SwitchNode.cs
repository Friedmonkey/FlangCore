using FriedLanguage.BuiltinType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FriedLanguage.Models.Parsing.Nodes
{
	internal class SwitchNode : SyntaxNode
	{
		public SwitchNode(SyntaxToken startTok) : base(startTok.Position, startTok.Position) { } // We expect the parser to properly define the endpos

		public List<SyntaxNode> Case { get; set; } = new();
		public List<SyntaxNode> Code { get; set; } = new();
		public SyntaxToken Check { get; set; } = default;

		public override NodeType Type => NodeType.Switch;

		public override FValue Evaluate(Scope scope)
		{
			var scope2 = new Scope(scope, StartPosition);
			scope2.ParentScope = scope;

			int defaultPos = -1;

			FValue jump = null;
			var val = scope.Get(Check.Text);
			Dictionary<FValue,SyntaxNode> Casues = new Dictionary<FValue,SyntaxNode>();
			SyntaxNode Default = null;
			foreach (var cas in Code)
			{
				if (cas is CaseNode cn)
				{
					Casues.Add(cn.Expr.Evaluate(scope), cn);
					scope2.SetJumpPosCreatedPos(scope.CreatedPosition);
					scope2.Set(cn.Name, new FLabel(cn.Position));
				}
				else if (cas is LabelNode ln)
				{
					if (ln.Name == "case_default")
					{ 
						Default = ln;
						scope2.Set(ln.Name, new FLabel(ln.Position));
					}
				}
			}
			int idx = 0;
			foreach (var (value,node) in Casues)
			{
				if (value.Equals(val).IsTruthy())
				{
					var tmp = Casues.ElementAt(idx);
					if (tmp.Value is CaseNode cn)
					{ 
						jump = scope2.Get(cn.Name);
						//scope2.Delete(cn.Name);
					}
					break;
				}
				idx++;
			}
			if (Default != null) 
			{
				var tmp = Default;
				if (tmp is LabelNode ln)
				{ 
					defaultPos = ln.Position;
                    if (jump == null)
						jump = scope2.Get(ln.Name);
					//scope2.Delete(ln.Name);
				}
			}
			else
			{
				if (jump == null)
				{
					throw new Exception("switch default case does not exist, add \"case default:\"");
				}
			}

			if (jump is FLabel fLabel)
			{
				EvaluateBlock(scope2,fLabel.Position,defaultPos);
				return new FNull();
			}
			return FValue.Null;
		}

		public FValue EvaluateBlock(Scope scope,int start = 0,int defaultPos = -1)
		{
			var lastVal = FValue.Null;
			var blockScope = scope;

			bool createNewScope = false;

			if (createNewScope) blockScope = new Scope(scope, StartPosition);

			for (int i = start; i < Code.Count(); i++)
			{
				var node = Code[i];
				//skip labels we already evaluated
				if (node is LabelNode)
					continue;
				if (node is CaseNode)
					continue;
				var res = node.Evaluate(blockScope);

				if (!res.IsNull())
				{
					lastVal = res;
				}

                if (scope.BreakAmount >= 1)
                {
                    scope.SetState(ScopeState.ShouldBreak);
                    scope.SetBreakAmount(scope.BreakAmount);
                }
                else if (scope.State == ScopeState.ShouldBreak)
                    scope.SetState(ScopeState.None);

                if (scope.UnmatchAmount >= 1)
                {
                    scope.SetState(ScopeState.ShouldUnmatch);
                    scope.SetUnmatchAmount(scope.UnmatchAmount);
                }
                else if (scope.State == ScopeState.ShouldUnmatch)
                    scope.SetState(ScopeState.None);

				if (scope.State == ScopeState.ShouldBreak)
				{ 
					scope.SetBreakAmount(scope.BreakAmount - 1);
					return lastVal;
				}
				if (scope.State == ScopeState.ShouldContinue) 
					return lastVal;

				if (scope.State == ScopeState.ShouldUnmatch)
				{
					if (defaultPos == -1)
					{
						throw new Exception("switch default case does not exist, add \"case default:\"");
					}
					else
					{ 
						i = defaultPos-1;
						scope.SetState(ScopeState.None);
						continue;
					}
                }

				if (blockScope.State == ScopeState.ShouldJump)
				{
					if (/*blockScope.CreatedPosition*/0 == blockScope.JumpToIfCreatedPosition)
					{
						i = blockScope.JumpTo - 1;
						scope.SetState(ScopeState.None);
						continue;
					}
					else //finish this code block
					{
						return lastVal;
					}
				}


				if (scope.State == ScopeState.ShouldReturn)
				{
					var v = scope.ReturnValue;
					return v;
				}
			}

			return lastVal;
		}

		public override IEnumerable<SyntaxNode> GetChildren()
		{
			foreach (var co in Code)
			{
				yield return co;
			}
		}

		internal void AddCase(SyntaxNode casee)
		{
			Case.Add(casee);
		}

		public override string ToString()
		{
			return "SwitchNode:";
		}
	}
}
