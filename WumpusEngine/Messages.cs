using System.Diagnostics.CodeAnalysis;

namespace WumpusEngine;

[ExcludeFromCodeCoverage]
public class Messages
{
    private readonly IRandom _random;

    public Messages(IRandom random)
    {
        _random = random;
    }

    public static string GetMealDescription()
    {
        var hour = DateTime.Now.Hour;

        if (hour < 1)
        {
            return " a midnight snack.";
        }
        if (hour < 6)
        {
            return " a late night snack.";
        }
        if (hour < 10)
        {
            return " breakfast.";
        }
        if (hour < 12)
        {
            return " brunch.";
        }
        if (hour < 16)
        {
            return " lunch.";
        }
        if (hour < 18)
        {
            return " an early dinner.";
        }
        if (hour < 21)
        {
            return " dinner.";
        }

        return " a snack.";
    }

    public string GetEatenDescription(bool batDropped)
    {
        if (batDropped)
        {
            return $"You have encountered a bat who picks you up and drops you next to the fearsome Wumpus. You are devoured for {Messages.GetMealDescription()}";
        }

        return _random.Next(4) switch
        {
            0 => "You see the fearsome Wumpus, but before you can draw your bow, you are devoured for" + GetMealDescription(),
            1 => "As you are distracted by the blood splattered all around, the fearsome Wumpus sneaks up from behind and devours you for" + GetMealDescription(),
            2 => "Without paying attention you've walked right into the fearsome Wumpus' lair. It devours you for" + GetMealDescription(),
            _ => "You stumble upon the fearsome Wumpus and are devoured for" + GetMealDescription(),
        };
    }

    public string GetPitDescription(bool batDropped)
    {
        if (batDropped)
        {
            return "You have encountered a bat who picks you up and drops you into a bottomless pit.";
        }

        return _random.Next(3) switch
        {
            0 => "You have fallen into a bottomless pit. At least you don't have the fearsome Wumpus to worry about anymore.",
            1 => "You trip into a bottomless pit. Unfortunately, there is no bat around to save you.",
            _ => "You have fallen into a bottomless pit. After falling for several hours, you think it might have been better to have been eaten by the fearsome Wumpus.",
        };
    }

    public string GetMissedDescription(Cavern cavern, Direction direction)
    {
        var noise = "he noise attracts the fearsome Wumpus and you are devoured for" + GetMealDescription();

        if (cavern[direction] == null)
        {
            return "You take careful aim and fire your arrow directly into a wall. T" + noise + " What were you thinking?";
        }

        if (!cavern.HasBlood)
        {
            return "The fearsome Wumpus wasn't there. In fact there was no sign the fearsome Wumpus was nearby. However, t" + noise;
        }

        return _random.Next(2) switch
        {
            0 => "Close but no cigar; maybe next time.  Only there won't be a next time because t" + noise,
            _ => "That wasn't where the fearsome Wumpus was. T" + noise,
        };
    }

    public string GetVictoryDescription()
    {
        return _random.Next(5) switch
        {
            0 => "You've killed the fearsome Wumpus. Doesn't seem so fearsome now.",
            1 => "You've killed the fearsome Wumpus. I hope you had a hunting license.",
            2 => "The fearsome Wumpus lies dead before you. Now how do you get out of here...",
            3 => "Your arrow strikes true and slays the fearsome Wumpus. You are hailed a hero and become a legend in your own time.",
            _ => GetRidiculousMessage(),
        };
    }

    private string GetRidiculousMessage()
    {
        string body = _random.Next(6) switch
        {
            0 => "black heart",
            1 => "misshapen head",
            2 => "thick neck",
            3 => "broad, muscular chest",
            4 => "distended belly",
            _ => "knee",
        };

        return $"With a twang from your bow, your trusty arrow streaks through the air and strikes the fearsome Wumpus in its {body}. The fearsome Wumpus bellows in pain and anguish before slumping to the ground. Its cries turn to whimpers and after a few minutes, the once fearsome Wumpus breathes one last time and then lies silent. {(body == "knee" ? "Really, the knee? Who knew that was the fearsome Wumpus' weak spot?" : "")}";
    }
}
