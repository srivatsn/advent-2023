using System.Text.RegularExpressions;

var input = File.ReadAllText(args.Length > 0 ? args[0] : "input.txt");
var lines = input.Split("\n");

var workflows = lines.TakeWhile(l => !string.IsNullOrWhiteSpace(l)).Select(Workflow.ParseWorkflow).ToDictionary(w => w.Name, w => w.Rules);
var ratings = lines.SkipWhile(l => !string.IsNullOrWhiteSpace(l)).Skip(1).Select(Rating.ParseRating).ToList();

// Part 1
var sum = 0;
foreach (var rating in ratings)
{
    var next = DoWorkflow("in", rating);
    if (next == "A")
    {
        sum += rating.X + rating.M + rating.A + rating.S;
    }
}
Console.WriteLine($"Sum of all ratings: {sum}");

// Part 2

long combinations = 0;
foreach (var c in GetPossibleAcceptPaths("in", Constraint.All))
{
    combinations += (long)(c.X.End - c.X.Start + 1) * (long)(c.M.End - c.M.Start + 1) * (long)(c.A.End - c.A.Start + 1) * (long)(c.S.End - c.S.Start + 1);
    Console.WriteLine(c);
}

Console.WriteLine($"Possible combinations: {combinations}");

string DoWorkflow(string workflowName, Rating rating)
{
    var workflow = workflows![workflowName];
    string nextWorkflow = "";
    foreach (var rule in workflow)
    {
        if (rule.Condition == null)
        {
            nextWorkflow = rule.NextWorkflow;
            break;
        }

        var condition = rule.Condition;
        var value = condition.Category switch
        {
            'x' => rating.X,
            'm' => rating.M,
            'a' => rating.A,
            's' => rating.S,
            _ => throw new Exception("Unknown category")
        };

        if (condition.Operator == '<' && value < condition.Value)
        {
            nextWorkflow = rule.NextWorkflow;
            break;
        }
        else if (condition.Operator == '>' && value > condition.Value)
        {
            nextWorkflow = rule.NextWorkflow;
            break;
        }
    }

    if (nextWorkflow == "A" || nextWorkflow == "R")
    {
        return nextWorkflow;
    }
    else
    {
        return DoWorkflow(nextWorkflow, rating);
    }
}


HashSet<Constraint> GetPossibleAcceptPaths(string workflowName, Constraint constraintForNextRule)
{
    var rules = workflows[workflowName];
    var constraintsThatLeadToAccept = new HashSet<Constraint>();
    foreach (var rule in rules)
    {
        if (rule.Condition != null)
        {
            var condition = rule.Condition;
            if (rule.NextWorkflow == "A")
            {
                constraintsThatLeadToAccept.Add(constraintForNextRule.Apply(condition));
            }
            else if (rule.NextWorkflow != "R")
            {
                foreach (var c in GetPossibleAcceptPaths(rule.NextWorkflow, constraintForNextRule.Apply(condition)))
                {
                    constraintsThatLeadToAccept.Add(c.Intersect(constraintForNextRule.Apply(condition)));
                }
            }

            var inverseCondition = InvertCondition(condition);
            constraintForNextRule = constraintForNextRule.Apply(inverseCondition);

        }
        else
        {
            if (rule.NextWorkflow == "A")
            {
                constraintsThatLeadToAccept.Add(constraintForNextRule);
            }
            else if (rule.NextWorkflow != "R")
            {
                foreach (var c in GetPossibleAcceptPaths(rule.NextWorkflow, constraintForNextRule))
                {
                    constraintsThatLeadToAccept.Add(c.Intersect(constraintForNextRule));
                }
            }
        }
    }

    return constraintsThatLeadToAccept;
}


static Condition InvertCondition(Condition condition)
{
    return condition.Operator switch
    {
        '<' => new Condition(condition.Category, '>', condition.Value - 1),
        '>' => new Condition(condition.Category, '<', condition.Value + 1),
        _ => throw new Exception("Unknown operator")
    };
}

record Workflow(string Name, List<Rule> Rules)
{
    public static Workflow ParseWorkflow(string input)
    {
        var parts = Regex.Match(input, @"(?<name>\w+)\{(?<rule>([^,\}]+,)*[^,\}]+)\}").Groups;
        var name = parts["name"].Value;
        var rules = parts["rule"].Value.Split(",").Select(Rule.ParseRule).ToList();
        return new Workflow(name, rules);
    }
}

record Rule(Condition? Condition, string NextWorkflow)
{
    public static Rule ParseRule(string input)
    {
        var parts = Regex.Match(input, @"(?<condition>[^:]+:)?(?<nextworkflow>\w+)").Groups;
        var nextWorkflow = parts["nextworkflow"].Value;
        var condition = parts["condition"].Value;
        Condition? cond = null;
        if (!string.IsNullOrWhiteSpace(condition))
        {
            cond = Condition.ParseCondition(condition);
        }
        return new Rule(cond, nextWorkflow);
    }
}

record Condition(char Category, char Operator, int Value)
{
    public int Value { get; set; } = Value;

    public static Condition ParseCondition(string input)
    {
        var parts = Regex.Match(input, @"(?<category>[xmas])(?<operator>[><])(?<value>\d+)").Groups;
        var category = parts["category"].Value[0];
        var op = parts["operator"].Value[0];
        var value = int.Parse(parts["value"].Value);
        return new Condition(category, op, value);
    }
}

record Rating(int X, int M, int A, int S)
{
    public static Rating ParseRating(string input)
    {
        var parts = Regex.Match(input, @"\{x=(?<x>\d+),m=(?<m>\d+),a=(?<a>\d+),s=(?<s>\d+)\}").Groups;
        var x = int.Parse(parts["x"].Value);
        var m = int.Parse(parts["m"].Value);
        var a = int.Parse(parts["a"].Value);
        var s = int.Parse(parts["s"].Value);
        return new Rating(x, m, a, s);
    }
}

record Range(int Start, int End);

record Constraint()
{
    public Range X { get; init; } = new Range(1, 4000);
    public Range M { get; init; } = new Range(1, 4000);
    public Range A { get; init; } = new Range(1, 4000);
    public Range S { get; init; } = new Range(1, 4000);

    public static Constraint All { get; } = new Constraint();
    public static Constraint Empty { get; } = new Constraint() with { X = new Range(0, 0), M = new Range(0, 0), A = new Range(0, 0), S = new Range(0, 0) };

    public Constraint Apply(Condition condition)
    {
        var rangeToUpdate = condition.Category switch
        {
            'x' => X,
            'm' => M,
            'a' => A,
            's' => S,
            _ => throw new Exception("Unknown category")
        };

        var newStart = rangeToUpdate.Start;
        var newEnd = rangeToUpdate.End;
        if (condition.Operator == '<')
        {
            newEnd = Math.Min(rangeToUpdate.End, condition.Value - 1);

        }
        else
        {
            newStart = Math.Max(rangeToUpdate.Start, condition.Value + 1);
        }

        return condition.Category switch
        {
            'x' => this with { X = X with { Start = newStart, End = newEnd } },
            'm' => this with { M = M with { Start = newStart, End = newEnd } },
            'a' => this with { A = A with { Start = newStart, End = newEnd } },
            's' => this with { S = S with { Start = newStart, End = newEnd } },
            _ => throw new Exception("Unknown category")
        };
    }

    public Constraint Intersect(Constraint other)
    {
        return this with
        {
            X = X with { Start = Math.Max(X.Start, other.X.Start), End = Math.Min(X.End, other.X.End) },
            M = M with { Start = Math.Max(M.Start, other.M.Start), End = Math.Min(M.End, other.M.End) },
            A = A with { Start = Math.Max(A.Start, other.A.Start), End = Math.Min(A.End, other.A.End) },
            S = S with { Start = Math.Max(S.Start, other.S.Start), End = Math.Min(S.End, other.S.End) },
        };
    }
}