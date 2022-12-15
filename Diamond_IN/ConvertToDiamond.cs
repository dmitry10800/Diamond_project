﻿using System.Collections.Generic;
using System.IO;
using Integration;

namespace Diamond_IN
{
    class ConvertToDiamond
    {
        public static List<Diamond.Core.Models.LegalStatusEvent> FerTableConvertation(List<ProcessFebTableData.ElementsForOutput> elementOuts)
        {
            /*list of record for whole gazette chapter*/
            List<Diamond.Core.Models.LegalStatusEvent> fullGazetteInfo = new List<Diamond.Core.Models.LegalStatusEvent>();
            if (elementOuts != null)
            {
                int leCounter = 1;
                /*Create a new event to fill*/
                foreach (var record in elementOuts)
                {
                    Diamond.Core.Models.LegalStatusEvent legalEvent = new Diamond.Core.Models.LegalStatusEvent();
                    string tmpNote = "";
                    legalEvent.GazetteName = Path.GetFileName(IN_main.CurrentFileName.Replace("*_TableFER.txt", ".pdf").Replace(".txt", ".pdf"));
                    /*Setting subcode*/
                    legalEvent.SubCode = "10";
                    /*Setting Section Code*/
                    legalEvent.SectionCode = "EC";
                    /*Setting Country Code*/
                    legalEvent.CountryCode = "IN";
                    /*Setting File Name*/
                    legalEvent.Id = leCounter++; // creating uniq identifier
                    LegalEvent legal = new LegalEvent();
                    Biblio biblioData = new Biblio();
                    biblioData.Application.Number = record.AppNumber;
                    legal.Number = record.AppNumber;

                    legal.Date = record.FerDate;

                    if (record.LocationName != null) tmpNote = string.Concat("|| ", tmpNote, "LOCATION", " | ", record.LocationName);
                    if (record.Email != null) tmpNote = string.Concat(tmpNote, " || ", "EMAIL", " | ", record.Email);
                    if (tmpNote != "") legal.Note = tmpNote;

                    if (record.agent != null)
                    {
                        biblioData.Agents = new List<PartyMember>();
                        PartyMember agent = new PartyMember();
                        agent.Name = record.agent.Name;
                        agent.Address1 = record.agent.Address;
                        agent.Country = record.agent.Country;
                        biblioData.Agents.Add(agent);
                    }
                    legal.Language = "EN";

                    legalEvent.Biblio = biblioData;
                    legalEvent.LegalEvent = legal;
                    fullGazetteInfo.Add(legalEvent);
                }
            }
            return fullGazetteInfo;
        }
    }
}
