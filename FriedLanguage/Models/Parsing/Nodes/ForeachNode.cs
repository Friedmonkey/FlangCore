﻿using FriedLanguage.BuiltinType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.Models.Parsing.Nodes
{
	internal class ForeachNode : SyntaxNode
	{
		private SyntaxToken iterator;
		private SyntaxNode list;
		private SyntaxNode block;

		public ForeachNode(SyntaxToken iterat, SyntaxNode list, SyntaxNode block) : base(block.StartPosition, block.EndPosition)
		{
			this.iterator = iterat;
			this.list = list;
			this.block = block;
		}

		public override NodeType Type => NodeType.Foreach;

		public override FValue Evaluate(Scope scope)
		{
			Scope foreachScope = new(scope, StartPosition);
            FValue lastVal = FValue.Null;

            var numerat = list.Evaluate(scope);

			if (numerat == null)
				throw new Exception($"Enumareble did not parse.");


			if (numerat is FList flist)
			{
				if (flist.Value.Count() > 0)
				{
					foreach (var item in flist.Value)
					{
						foreachScope.SetAdmin(iterator.Text, item);
                        var foreachBlockRes = block.Evaluate(foreachScope);

                        if (!foreachBlockRes.IsNull()) lastVal = foreachBlockRes;

                        if (foreachScope.State == ScopeState.ShouldBreak) break;
                        if (foreachScope.State == ScopeState.ShouldJump) break;
                        if (foreachScope.State != ScopeState.None) foreachScope.SetState(ScopeState.None);
                    }
                    if (foreachScope.State == ScopeState.ShouldBreak)
                    {
                        foreachScope.SetBreakAmount(foreachScope.BreakAmount - 1);
                    }
                }
				else
					return new FNull();
			}
			else if (numerat is FDictionary fdict)
			{
				if (fdict.Value.Count() > 0)
				{
					foreach (var item in fdict.Value)
					{
						foreachScope.SetAdmin(iterator.Text, item.val);
                        var foreachBlockRes = block.Evaluate(foreachScope);

                        if (!foreachBlockRes.IsNull()) lastVal = foreachBlockRes;

                        if (foreachScope.State == ScopeState.ShouldBreak) break;
                        if (foreachScope.State == ScopeState.ShouldJump) break;
                        if (foreachScope.State != ScopeState.None) foreachScope.SetState(ScopeState.None);
                    }
                    if (foreachScope.State == ScopeState.ShouldBreak)
                    {
                        foreachScope.SetBreakAmount(foreachScope.BreakAmount - 1);
                    }
                }
				else
					return new FNull();
			}
			else
				throw new Exception("Could not parse to enumareble.");

			return new FNull();
		}

		public override IEnumerable<SyntaxNode> GetChildren()
		{
			yield return block;
		}
	}
}
