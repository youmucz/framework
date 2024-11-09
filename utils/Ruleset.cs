using System;
using System.Collections;
using System.Collections.Generic;

namespace Minikit
{
    /// <summary> Rulesets are a list of functions that return either true or false. The intention is for other 
    /// objects to add/remove rules to a ruleset, and the object that owns the ruleset determines when to check 
    /// the ruleset. </summary>
    public class Ruleset<T>
    {
        public delegate bool Rule(T generic);

        private readonly List<Rule> _rules = new();

        /// <summary> Checks if all the rules passed </summary>
        public bool Check(T generic)
        {
            foreach (var rule in _rules)
            {
                if (rule != null)
                {
                    if (!rule.Invoke(generic))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool AddRule(Rule rule)
        {
            if (_rules.Contains(rule))
            {
                return false;
            }

            _rules.Add(rule);
            return true;
        }

        public bool RemoveRule(Rule rule)
        {
            if (!_rules.Contains(rule))
            {
                return false;
            }

            _rules.Remove(rule);
            return true;
        }
    }
}
