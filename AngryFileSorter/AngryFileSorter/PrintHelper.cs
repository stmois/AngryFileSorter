using System;
using System.Net.Sockets;

namespace AngryFileSorter;

public static class PrintHelper
{
    public static void Print(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ForegroundColor = ConsoleColor.White;

        
    }

    public void SendResponse()
    {
        var inviteToPivnuha = GetRequest(requestor: Mikhail);
        ProccessRequest(inviteToPivnuha);
    }

    public bool ProccessRequest(PivnuhaRequest inviteToPivnuha)
    {
        if (inviteToPivnuha.Status == ReceivedFromMikhail)
        {
            var acceptedBySelfMatters = await CheckSelfMatters(AcceptIfEmpty);
            var dayType = CheckDay(DateTime.Now);

            if (acceptedBySelfMatters && dayType is { holiday, weekend, vacation }) // true
            {
                SendAcceptedAnswer(receiver: Mikhail);
                SendMeetingTime(receiver: realPacany, TimeOnly: 21:00);
            }
            else
            {
                throw new BadAnswerException(MessageProcessingHandler: "lets try other day");
            }
        }
    }
}
