#region License
/* 
 * Copyright (C) 1999-2018 John K�ll�n.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;

namespace Reko.Analysis
{
	/// <summary>
	/// Chases a chain of statements to locate the expression that
	/// defines the value of a condition code.
	/// </summary>
	public class GrfDefinitionFinder : InstructionVisitorBase
	{
		private SsaIdentifierCollection ssaIds;
		private SsaIdentifier sid;
		private bool negated;
		private Statement stm;
		private Expression defExpr;

		public GrfDefinitionFinder(SsaIdentifierCollection ssaIds)
		{
			this.ssaIds = ssaIds;
		}

		/// <summary>
		/// Chases a chain statements to locate the expression that
		/// defines the value of a condition code.
		/// </summary>
		/// <param name="sid"></param>
		/// <returns></returns>
		public void FindDefiningExpression(SsaIdentifier sid)
		{
			this.sid = sid;
			negated = false;
			stm = sid.DefStatement;
			if (stm != null)
			{
				Statement stmOld = null;
				defExpr = null;
				while (stm != null && defExpr == null)
				{
					stmOld = stm;
					stm = null;
					stmOld.Instruction.Accept(this);
				}
			}
		}

		public Expression DefiningExpression
		{
			get { return defExpr; }
		}

		public bool IsNegated
		{
			get { return negated; }
		}

		public override void VisitApplication(Application appl)
		{
			defExpr = appl;
		}

		public override void VisitAssignment(Assignment a)
		{
			a.Src.Accept(this);
		}

        public override void VisitCallInstruction(CallInstruction ci)
        {
            foreach (var di in ci.Definitions)
            {
                if (di.Expression == sid.Identifier)
                {
                    defExpr = new Application(ci.Callee, new UnknownType());
                    return;
                }
            }
        }

		public override void VisitIdentifier(Identifier id)
		{
			sid = ssaIds[id];
			stm = sid.DefStatement;
		}

		public override void VisitBinaryExpression(BinaryExpression binExp)
		{
			defExpr = binExp;
		}

		public override void VisitConditionOf(ConditionOf cof)
		{
			defExpr = cof;
		}

		public override void VisitDefInstruction(DefInstruction def)
		{
		}
		
		public override void VisitPhiFunction(PhiFunction phi)
		{
			defExpr = phi;
		}

        public override void VisitSlice(Slice slice)
        {
            base.VisitSlice(slice);
        }

        public override void VisitUnaryExpression(UnaryExpression unary)
		{
			if (unary != null && unary.Operator == Operator.Not)
			{
				Identifier id = (Identifier) unary.Expression;
				negated = !negated;

				stm = ssaIds[id].DefStatement;
			}
		}
	}
}
