﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

namespace Diamond_ID
{
    class Methods
    {
        private static readonly string I71 = "(71) Nama dan Alamat yang Mengajukan Permohonan Paten :";
        private static readonly string I72 = "(72) Nama Inventor :";
        private static readonly string I74 = "(74) Nama dan Alamat Konsultan Paten :";
        public static string GetCountryCode(string s)
        {
            switch (s)
            {
                case var country when new Regex(@"African Intellectual Property Organization", RegexOptions.IgnoreCase).Match(country).Success: return "OA";
                case var country when new Regex(@"Albania", RegexOptions.IgnoreCase).Match(country).Success: return "AL";
                case var country when new Regex(@"Algeria", RegexOptions.IgnoreCase).Match(country).Success: return "DZ";
                case var country when new Regex(@"Andorra", RegexOptions.IgnoreCase).Match(country).Success: return "AD";
                case var country when new Regex(@"Angola", RegexOptions.IgnoreCase).Match(country).Success: return "AO";
                case var country when new Regex(@"Argentina", RegexOptions.IgnoreCase).Match(country).Success: return "AR";
                case var country when new Regex(@"Armenia", RegexOptions.IgnoreCase).Match(country).Success: return "AM";
                case var country when new Regex(@"AsiaPacific Region", RegexOptions.IgnoreCase).Match(country).Success: return "AP";
                case var country when new Regex(@"Australia", RegexOptions.IgnoreCase).Match(country).Success: return "AU";
                case var country when new Regex(@"Austria", RegexOptions.IgnoreCase).Match(country).Success: return "AT";
                case var country when new Regex(@"Azerbeidzjan", RegexOptions.IgnoreCase).Match(country).Success: return "AZ";
                case var country when new Regex(@"Bahamas", RegexOptions.IgnoreCase).Match(country).Success: return "BS";
                case var country when new Regex(@"Bangladesh", RegexOptions.IgnoreCase).Match(country).Success: return "BD";
                case var country when new Regex(@"Barbados", RegexOptions.IgnoreCase).Match(country).Success: return "BB";
                case var country when new Regex(@"Belarus", RegexOptions.IgnoreCase).Match(country).Success: return "BY";
                case var country when new Regex(@"Belgium", RegexOptions.IgnoreCase).Match(country).Success: return "BE";
                case var country when new Regex(@"Belize", RegexOptions.IgnoreCase).Match(country).Success: return "BZ";
                case var country when new Regex(@"Bhutan", RegexOptions.IgnoreCase).Match(country).Success: return "BT";
                case var country when new Regex(@"Bolivia", RegexOptions.IgnoreCase).Match(country).Success: return "BO";
                case var country when new Regex(@"Bosnia And Herzegovina", RegexOptions.IgnoreCase).Match(country).Success: return "BA";
                case var country when new Regex(@"Botswana", RegexOptions.IgnoreCase).Match(country).Success: return "BW";
                case var country when new Regex(@"Brasil", RegexOptions.IgnoreCase).Match(country).Success: return "BR";
                case var country when new Regex(@"Brunei Darussalam", RegexOptions.IgnoreCase).Match(country).Success: return "BN";
                case var country when new Regex(@"Bulgaria", RegexOptions.IgnoreCase).Match(country).Success: return "BG";
                case var country when new Regex(@"Canada", RegexOptions.IgnoreCase).Match(country).Success: return "CA";
                case var country when new Regex(@"Cape Verde", RegexOptions.IgnoreCase).Match(country).Success: return "CV";
                case var country when new Regex(@"Cayman Islands", RegexOptions.IgnoreCase).Match(country).Success: return "KY";
                case var country when new Regex(@"Chili", RegexOptions.IgnoreCase).Match(country).Success: return "CL";
                case var country when new Regex(@"China", RegexOptions.IgnoreCase).Match(country).Success: return "CN";
                case var country when new Regex(@"Colombia", RegexOptions.IgnoreCase).Match(country).Success: return "CO";
                case var country when new Regex(@"CostaRica", RegexOptions.IgnoreCase).Match(country).Success: return "CR";
                case var country when new Regex(@"Croatia", RegexOptions.IgnoreCase).Match(country).Success: return "HR";
                case var country when new Regex(@"Cuba", RegexOptions.IgnoreCase).Match(country).Success: return "CU";
                case var country when new Regex(@"Czech", RegexOptions.IgnoreCase).Match(country).Success: return "CZ";
                case var country when new Regex(@"Czechoslovakia", RegexOptions.IgnoreCase).Match(country).Success: return "CS";
                case var country when new Regex(@"Denmark", RegexOptions.IgnoreCase).Match(country).Success: return "DK";
                case var country when new Regex(@"Djibouti", RegexOptions.IgnoreCase).Match(country).Success: return "DJ";
                case var country when new Regex(@"Dominica", RegexOptions.IgnoreCase).Match(country).Success: return "DM";
                case var country when new Regex(@"Dominican Republic", RegexOptions.IgnoreCase).Match(country).Success: return "DO";
                case var country when new Regex(@"Eapo", RegexOptions.IgnoreCase).Match(country).Success: return "EA";
                case var country when new Regex(@"Ecuador", RegexOptions.IgnoreCase).Match(country).Success: return "EC";
                case var country when new Regex(@"Egypt", RegexOptions.IgnoreCase).Match(country).Success: return "EG";
                case var country when new Regex(@"El Salvador", RegexOptions.IgnoreCase).Match(country).Success: return "SV";
                case var country when new Regex(@"Epo", RegexOptions.IgnoreCase).Match(country).Success: return "EP";
                case var country when new Regex(@"Estonia", RegexOptions.IgnoreCase).Match(country).Success: return "EE";
                case var country when new Regex(@"Ethiopia", RegexOptions.IgnoreCase).Match(country).Success: return "ET";
                case var country when new Regex(@"Fiji", RegexOptions.IgnoreCase).Match(country).Success: return "FJ";
                case var country when new Regex(@"Finland", RegexOptions.IgnoreCase).Match(country).Success: return "FI";
                case var country when new Regex(@"France", RegexOptions.IgnoreCase).Match(country).Success: return "FR";
                case var country when new Regex(@"German Democratic Republic", RegexOptions.IgnoreCase).Match(country).Success: return "DD";
                case var country when new Regex(@"Germany", RegexOptions.IgnoreCase).Match(country).Success: return "DE";
                case var country when new Regex(@"Ghana", RegexOptions.IgnoreCase).Match(country).Success: return "GH";
                case var country when new Regex(@"GreatBritain", RegexOptions.IgnoreCase).Match(country).Success: return "GB";
                case var country when new Regex(@"Greece", RegexOptions.IgnoreCase).Match(country).Success: return "GR";
                case var country when new Regex(@"Guatemala", RegexOptions.IgnoreCase).Match(country).Success: return "GT";
                case var country when new Regex(@"Gulf Cooperation Council", RegexOptions.IgnoreCase).Match(country).Success: return "GC";
                case var country when new Regex(@"Guyana", RegexOptions.IgnoreCase).Match(country).Success: return "GY";
                case var country when new Regex(@"Honduras", RegexOptions.IgnoreCase).Match(country).Success: return "HN";
                case var country when new Regex(@"HongKong", RegexOptions.IgnoreCase).Match(country).Success: return "HK";
                case var country when new Regex(@"Hungary", RegexOptions.IgnoreCase).Match(country).Success: return "HU";
                case var country when new Regex(@"Iceland", RegexOptions.IgnoreCase).Match(country).Success: return "IS";
                case var country when new Regex(@"India", RegexOptions.IgnoreCase).Match(country).Success: return "IN";
                case var country when new Regex(@"Indonesia", RegexOptions.IgnoreCase).Match(country).Success: return "ID";
                case var country when new Regex(@"Iraq", RegexOptions.IgnoreCase).Match(country).Success: return "IQ";
                case var country when new Regex(@"Ireland", RegexOptions.IgnoreCase).Match(country).Success: return "IE";
                case var country when new Regex(@"Israel", RegexOptions.IgnoreCase).Match(country).Success: return "IL";
                case var country when new Regex(@"Italy", RegexOptions.IgnoreCase).Match(country).Success: return "IT";
                case var country when new Regex(@"Japan", RegexOptions.IgnoreCase).Match(country).Success: return "JP";
                case var country when new Regex(@"Jordan", RegexOptions.IgnoreCase).Match(country).Success: return "JO";
                case var country when new Regex(@"Kenya", RegexOptions.IgnoreCase).Match(country).Success: return "KE";
                case var country when new Regex(@"Korea", RegexOptions.IgnoreCase).Match(country).Success: return "KR";
                case var country when new Regex(@"Kyrgyzstan", RegexOptions.IgnoreCase).Match(country).Success: return "KG";
                case var country when new Regex(@"Latvia", RegexOptions.IgnoreCase).Match(country).Success: return "LV";
                case var country when new Regex(@"Lebanon", RegexOptions.IgnoreCase).Match(country).Success: return "LB";
                case var country when new Regex(@"Lithuania", RegexOptions.IgnoreCase).Match(country).Success: return "LT";
                case var country when new Regex(@"Luxemburg", RegexOptions.IgnoreCase).Match(country).Success: return "LU";
                case var country when new Regex(@"Macedonia", RegexOptions.IgnoreCase).Match(country).Success: return "MK";
                case var country when new Regex(@"Madagascar", RegexOptions.IgnoreCase).Match(country).Success: return "MG";
                case var country when new Regex(@"Malaysia", RegexOptions.IgnoreCase).Match(country).Success: return "MY";
                case var country when new Regex(@"Mauritius", RegexOptions.IgnoreCase).Match(country).Success: return "MU";
                case var country when new Regex(@"Mexico", RegexOptions.IgnoreCase).Match(country).Success: return "MX";
                case var country when new Regex(@"Monaco", RegexOptions.IgnoreCase).Match(country).Success: return "MC";
                case var country when new Regex(@"Mongolia", RegexOptions.IgnoreCase).Match(country).Success: return "MN";
                case var country when new Regex(@"Montenegro", RegexOptions.IgnoreCase).Match(country).Success: return "ME";
                case var country when new Regex(@"Montserrat", RegexOptions.IgnoreCase).Match(country).Success: return "MS";
                case var country when new Regex(@"Morocco", RegexOptions.IgnoreCase).Match(country).Success: return "MA";
                case var country when new Regex(@"Mozambique", RegexOptions.IgnoreCase).Match(country).Success: return "MZ";
                case var country when new Regex(@"Myanmar Burma", RegexOptions.IgnoreCase).Match(country).Success: return "MM";
                case var country when new Regex(@"Moldova", RegexOptions.IgnoreCase).Match(country).Success: return "MD";
                case var country when new Regex(@"Netherlands", RegexOptions.IgnoreCase).Match(country).Success: return "NL";
                case var country when new Regex(@"New Zealand", RegexOptions.IgnoreCase).Match(country).Success: return "NZ";
                case var country when new Regex(@"Norway", RegexOptions.IgnoreCase).Match(country).Success: return "NO";
                case var country when new Regex(@"Oman", RegexOptions.IgnoreCase).Match(country).Success: return "OM";
                case var country when new Regex(@"Pakistan", RegexOptions.IgnoreCase).Match(country).Success: return "PK";
                case var country when new Regex(@"Palestina", RegexOptions.IgnoreCase).Match(country).Success: return "PS";
                case var country when new Regex(@"Panama", RegexOptions.IgnoreCase).Match(country).Success: return "PA";
                case var country when new Regex(@"Papua New Guinea", RegexOptions.IgnoreCase).Match(country).Success: return "PG";
                case var country when new Regex(@"Paraguay", RegexOptions.IgnoreCase).Match(country).Success: return "PY";
                case var country when new Regex(@"Philippines", RegexOptions.IgnoreCase).Match(country).Success: return "PH";
                case var country when new Regex(@"Poland", RegexOptions.IgnoreCase).Match(country).Success: return "PL";
                case var country when new Regex(@"Portugal", RegexOptions.IgnoreCase).Match(country).Success: return "PT";
                case var country when new Regex(@"Qatar", RegexOptions.IgnoreCase).Match(country).Success: return "QA";
                case var country when new Regex(@"Republic Of Kosovo", RegexOptions.IgnoreCase).Match(country).Success: return "XK";
                case var country when new Regex(@"Romania", RegexOptions.IgnoreCase).Match(country).Success: return "RO";
                case var country when new Regex(@"Russia", RegexOptions.IgnoreCase).Match(country).Success: return "RU";
                case var country when new Regex(@"Rwanda", RegexOptions.IgnoreCase).Match(country).Success: return "RW";
                case var country when new Regex(@"Saint Vincen tAnd The Grenadines", RegexOptions.IgnoreCase).Match(country).Success: return "VC";
                case var country when new Regex(@"San Marino", RegexOptions.IgnoreCase).Match(country).Success: return "SM";
                case var country when new Regex(@"Saudi Arabia", RegexOptions.IgnoreCase).Match(country).Success: return "SA";
                case var country when new Regex(@"(Singapore)|(Singapura)", RegexOptions.IgnoreCase).Match(country).Success: return "SG";
                case var country when new Regex(@"Slovakia", RegexOptions.IgnoreCase).Match(country).Success: return "SK";
                case var country when new Regex(@"Slovenia", RegexOptions.IgnoreCase).Match(country).Success: return "SI";
                case var country when new Regex(@"South Africa", RegexOptions.IgnoreCase).Match(country).Success: return "ZA";
                case var country when new Regex(@"Soviet Union", RegexOptions.IgnoreCase).Match(country).Success: return "SU";
                case var country when new Regex(@"Spain", RegexOptions.IgnoreCase).Match(country).Success: return "ES";
                case var country when new Regex(@"Sri Lanka", RegexOptions.IgnoreCase).Match(country).Success: return "LK";
                case var country when new Regex(@"Sudan", RegexOptions.IgnoreCase).Match(country).Success: return "SD";
                case var country when new Regex(@"Sweden", RegexOptions.IgnoreCase).Match(country).Success: return "SE";
                case var country when new Regex(@"Switzerland", RegexOptions.IgnoreCase).Match(country).Success: return "CH";
                case var country when new Regex(@"Syria", RegexOptions.IgnoreCase).Match(country).Success: return "SY";
                case var country when new Regex(@"Taiwan", RegexOptions.IgnoreCase).Match(country).Success: return "TW";
                case var country when new Regex(@"Tajikistan", RegexOptions.IgnoreCase).Match(country).Success: return "TJ";
                case var country when new Regex(@"Tanzania", RegexOptions.IgnoreCase).Match(country).Success: return "TZ";
                case var country when new Regex(@"Thailand", RegexOptions.IgnoreCase).Match(country).Success: return "TH";
                case var country when new Regex(@"Trinidad And Tobago", RegexOptions.IgnoreCase).Match(country).Success: return "TT";
                case var country when new Regex(@"Tunisia", RegexOptions.IgnoreCase).Match(country).Success: return "TN";
                case var country when new Regex(@"Turkey", RegexOptions.IgnoreCase).Match(country).Success: return "TR";
                case var country when new Regex(@"Turkmenistan", RegexOptions.IgnoreCase).Match(country).Success: return "TM";
                case var country when new Regex(@"Ukraine", RegexOptions.IgnoreCase).Match(country).Success: return "UA";
                case var country when new Regex(@"United Arab Emirates", RegexOptions.IgnoreCase).Match(country).Success: return "AE";
                case var country when new Regex(@"United Kingdom", RegexOptions.IgnoreCase).Match(country).Success: return "GB";
                case var country when new Regex(@"(United States)|(E.U.A.)|(Estados Unidos)", RegexOptions.IgnoreCase).Match(country).Success: return "US";
                case var country when new Regex(@"Uruguay", RegexOptions.IgnoreCase).Match(country).Success: return "UY";
                case var country when new Regex(@"Uzbekistan", RegexOptions.IgnoreCase).Match(country).Success: return "UZ";
                case var country when new Regex(@"Venezuela", RegexOptions.IgnoreCase).Match(country).Success: return "VE";
                case var country when new Regex(@"Viet nam", RegexOptions.IgnoreCase).Match(country).Success: return "VN";
                case var country when new Regex(@"Wipo", RegexOptions.IgnoreCase).Match(country).Success: return "WO";
                case var country when new Regex(@"Yemen", RegexOptions.IgnoreCase).Match(country).Success: return "YE";
                case var country when new Regex(@"Zambia", RegexOptions.IgnoreCase).Match(country).Success: return "ZM";
                case var country when new Regex(@"Zimbabwe", RegexOptions.IgnoreCase).Match(country).Success: return "ZW";
                case var country when new Regex(@"Cyprus", RegexOptions.IgnoreCase).Match(country).Success: return "CY";
                case var country when new Regex(@"Albânia", RegexOptions.IgnoreCase).Match(country).Success: return "AL";
                case var country when new Regex(@"Armênia", RegexOptions.IgnoreCase).Match(country).Success: return "AM";
                case var country when new Regex(@"Angola", RegexOptions.IgnoreCase).Match(country).Success: return "AO";
                case var country when new Regex(@"Antártica", RegexOptions.IgnoreCase).Match(country).Success: return "AQ";
                case var country when new Regex(@"Argentina", RegexOptions.IgnoreCase).Match(country).Success: return "AR";
                case var country when new Regex(@"Samoa Americana", RegexOptions.IgnoreCase).Match(country).Success: return "AS";
                case var country when new Regex(@"Áustria", RegexOptions.IgnoreCase).Match(country).Success: return "AT";
                case var country when new Regex(@"Austrália", RegexOptions.IgnoreCase).Match(country).Success: return "AU";
                case var country when new Regex(@"Aruba", RegexOptions.IgnoreCase).Match(country).Success: return "AW";
                case var country when new Regex(@"Ilhas Aland", RegexOptions.IgnoreCase).Match(country).Success: return "AX";
                case var country when new Regex(@"Azerbaijão", RegexOptions.IgnoreCase).Match(country).Success: return "AZ";
                case var country when new Regex(@"Bósnia e Herzegovina", RegexOptions.IgnoreCase).Match(country).Success: return "BA";
                case var country when new Regex(@"Barbados", RegexOptions.IgnoreCase).Match(country).Success: return "BB";
                case var country when new Regex(@"Bangladesh", RegexOptions.IgnoreCase).Match(country).Success: return "BD";
                case var country when new Regex(@"Bélgica", RegexOptions.IgnoreCase).Match(country).Success: return "BE";
                case var country when new Regex(@"Burkina Faso", RegexOptions.IgnoreCase).Match(country).Success: return "BF";
                case var country when new Regex(@"Bulgária", RegexOptions.IgnoreCase).Match(country).Success: return "BG";
                case var country when new Regex(@"Barém", RegexOptions.IgnoreCase).Match(country).Success: return "BH";
                case var country when new Regex(@"Burundi", RegexOptions.IgnoreCase).Match(country).Success: return "BI";
                case var country when new Regex(@"Benin", RegexOptions.IgnoreCase).Match(country).Success: return "BJ";
                case var country when new Regex(@"São Bartolomeu", RegexOptions.IgnoreCase).Match(country).Success: return "BL";
                case var country when new Regex(@"Bermudas", RegexOptions.IgnoreCase).Match(country).Success: return "BM";
                case var country when new Regex(@"Brunei Darussalam", RegexOptions.IgnoreCase).Match(country).Success: return "BN";
                case var country when new Regex(@"Bolívia (Estado Plurinacional da)", RegexOptions.IgnoreCase).Match(country).Success: return "BO";
                case var country when new Regex(@"Bonaire, Santo Eustáquio e Saba", RegexOptions.IgnoreCase).Match(country).Success: return "BQ";
                case var country when new Regex(@"Brasil", RegexOptions.IgnoreCase).Match(country).Success: return "BR";
                case var country when new Regex(@"Bahamas", RegexOptions.IgnoreCase).Match(country).Success: return "BS";
                case var country when new Regex(@"Butão", RegexOptions.IgnoreCase).Match(country).Success: return "BT";
                case var country when new Regex(@"Ilha Bouvet", RegexOptions.IgnoreCase).Match(country).Success: return "BV";
                case var country when new Regex(@"Botsuana", RegexOptions.IgnoreCase).Match(country).Success: return "BW";
                case var country when new Regex(@"Bielorrússia", RegexOptions.IgnoreCase).Match(country).Success: return "BY";
                case var country when new Regex(@"Belize", RegexOptions.IgnoreCase).Match(country).Success: return "BZ";
                case var country when new Regex(@"Canadá", RegexOptions.IgnoreCase).Match(country).Success: return "CA";
                case var country when new Regex(@"Ilhas Cocos (Keeling)", RegexOptions.IgnoreCase).Match(country).Success: return "CC";
                case var country when new Regex(@"Congo, República Democrática do", RegexOptions.IgnoreCase).Match(country).Success: return "CD";
                case var country when new Regex(@"República Centro-Africana", RegexOptions.IgnoreCase).Match(country).Success: return "CF";
                case var country when new Regex(@"Congo", RegexOptions.IgnoreCase).Match(country).Success: return "CG";
                case var country when new Regex(@"Suíça", RegexOptions.IgnoreCase).Match(country).Success: return "CH";
                case var country when new Regex(@"Costa do Marfim", RegexOptions.IgnoreCase).Match(country).Success: return "CI";
                case var country when new Regex(@"Ilhas Cook", RegexOptions.IgnoreCase).Match(country).Success: return "CK";
                case var country when new Regex(@"Chile", RegexOptions.IgnoreCase).Match(country).Success: return "CL";
                case var country when new Regex(@"Camarões", RegexOptions.IgnoreCase).Match(country).Success: return "CM";
                case var country when new Regex(@"China", RegexOptions.IgnoreCase).Match(country).Success: return "CN";
                case var country when new Regex(@"Colômbia", RegexOptions.IgnoreCase).Match(country).Success: return "CO";
                case var country when new Regex(@"Costa Rica", RegexOptions.IgnoreCase).Match(country).Success: return "CR";
                case var country when new Regex(@"Cuba", RegexOptions.IgnoreCase).Match(country).Success: return "CU";
                case var country when new Regex(@"Cabo Verde", RegexOptions.IgnoreCase).Match(country).Success: return "CV";
                case var country when new Regex(@"Curaçao", RegexOptions.IgnoreCase).Match(country).Success: return "CW";
                case var country when new Regex(@"Ilha do Natal", RegexOptions.IgnoreCase).Match(country).Success: return "CX";
                case var country when new Regex(@"Chipre", RegexOptions.IgnoreCase).Match(country).Success: return "CY";
                case var country when new Regex(@"Czechia", RegexOptions.IgnoreCase).Match(country).Success: return "CZ";
                case var country when new Regex(@"Alemanha", RegexOptions.IgnoreCase).Match(country).Success: return "DE";
                case var country when new Regex(@"Djibuti", RegexOptions.IgnoreCase).Match(country).Success: return "DJ";
                case var country when new Regex(@"Dinamarca", RegexOptions.IgnoreCase).Match(country).Success: return "DK";
                case var country when new Regex(@"Dominica", RegexOptions.IgnoreCase).Match(country).Success: return "DM";
                case var country when new Regex(@"República Dominicana", RegexOptions.IgnoreCase).Match(country).Success: return "DO";
                case var country when new Regex(@"Argélia", RegexOptions.IgnoreCase).Match(country).Success: return "DZ";
                case var country when new Regex(@"Equador", RegexOptions.IgnoreCase).Match(country).Success: return "EC";
                case var country when new Regex(@"Estônia", RegexOptions.IgnoreCase).Match(country).Success: return "EE";
                case var country when new Regex(@"Egito", RegexOptions.IgnoreCase).Match(country).Success: return "EG";
                case var country when new Regex(@"Saara Ocidental", RegexOptions.IgnoreCase).Match(country).Success: return "EH";
                case var country when new Regex(@"Eritreia", RegexOptions.IgnoreCase).Match(country).Success: return "ER";
                case var country when new Regex(@"Espanha", RegexOptions.IgnoreCase).Match(country).Success: return "ES";
                case var country when new Regex(@"Etiópia", RegexOptions.IgnoreCase).Match(country).Success: return "ET";
                case var country when new Regex(@"Finlândia", RegexOptions.IgnoreCase).Match(country).Success: return "FI";
                case var country when new Regex(@"Fiji", RegexOptions.IgnoreCase).Match(country).Success: return "FJ";
                case var country when new Regex(@"Ilhas Falkland (Malvinas)", RegexOptions.IgnoreCase).Match(country).Success: return "FK";
                case var country when new Regex(@"Micronésia (Estados Federados da)", RegexOptions.IgnoreCase).Match(country).Success: return "FM";
                case var country when new Regex(@"ilhas Faroe", RegexOptions.IgnoreCase).Match(country).Success: return "FO";
                case var country when new Regex(@"França", RegexOptions.IgnoreCase).Match(country).Success: return "FR";
                case var country when new Regex(@"Gabão", RegexOptions.IgnoreCase).Match(country).Success: return "GA";
                case var country when new Regex(@"Reino Unido da Grã-Bretanha e Irlanda do Norte", RegexOptions.IgnoreCase).Match(country).Success: return "GB";
                case var country when new Regex(@"Granada", RegexOptions.IgnoreCase).Match(country).Success: return "GD";
                case var country when new Regex(@"Geórgia", RegexOptions.IgnoreCase).Match(country).Success: return "GE";
                case var country when new Regex(@"Guiana Francesa", RegexOptions.IgnoreCase).Match(country).Success: return "GF";
                case var country when new Regex(@"Guernsey", RegexOptions.IgnoreCase).Match(country).Success: return "GG";
                case var country when new Regex(@"Gana", RegexOptions.IgnoreCase).Match(country).Success: return "GH";
                case var country when new Regex(@"Gibraltar", RegexOptions.IgnoreCase).Match(country).Success: return "GI";
                case var country when new Regex(@"Gronelândia", RegexOptions.IgnoreCase).Match(country).Success: return "GL";
                case var country when new Regex(@"Gâmbia", RegexOptions.IgnoreCase).Match(country).Success: return "GM";
                case var country when new Regex(@"Guiné", RegexOptions.IgnoreCase).Match(country).Success: return "GN";
                case var country when new Regex(@"Guadalupe", RegexOptions.IgnoreCase).Match(country).Success: return "GP";
                case var country when new Regex(@"Guiné Equatorial", RegexOptions.IgnoreCase).Match(country).Success: return "GQ";
                case var country when new Regex(@"Grécia", RegexOptions.IgnoreCase).Match(country).Success: return "GR";
                case var country when new Regex(@"Ilhas Geórgia do Sul e Sandwich do Sul", RegexOptions.IgnoreCase).Match(country).Success: return "GS";
                case var country when new Regex(@"Guatemala", RegexOptions.IgnoreCase).Match(country).Success: return "GT";
                case var country when new Regex(@"Guam", RegexOptions.IgnoreCase).Match(country).Success: return "GU";
                case var country when new Regex(@"Guiné-Bissau", RegexOptions.IgnoreCase).Match(country).Success: return "GW";
                case var country when new Regex(@"Guiana", RegexOptions.IgnoreCase).Match(country).Success: return "GY";
                case var country when new Regex(@"Hong Kong", RegexOptions.IgnoreCase).Match(country).Success: return "HK";
                case var country when new Regex(@"Ilha Heard e Ilhas McDonald", RegexOptions.IgnoreCase).Match(country).Success: return "HM";
                case var country when new Regex(@"Honduras", RegexOptions.IgnoreCase).Match(country).Success: return "HN";
                case var country when new Regex(@"Croácia", RegexOptions.IgnoreCase).Match(country).Success: return "HR";
                case var country when new Regex(@"Haiti", RegexOptions.IgnoreCase).Match(country).Success: return "HT";
                case var country when new Regex(@"Hungria", RegexOptions.IgnoreCase).Match(country).Success: return "HU";
                case var country when new Regex(@"Indonésia", RegexOptions.IgnoreCase).Match(country).Success: return "ID";
                case var country when new Regex(@"Irlanda", RegexOptions.IgnoreCase).Match(country).Success: return "IE";
                case var country when new Regex(@"Israel", RegexOptions.IgnoreCase).Match(country).Success: return "IL";
                case var country when new Regex(@"Ilha de Man", RegexOptions.IgnoreCase).Match(country).Success: return "IM";
                case var country when new Regex(@"Índia", RegexOptions.IgnoreCase).Match(country).Success: return "IN";
                case var country when new Regex(@"Território Britânico do Oceano Índico", RegexOptions.IgnoreCase).Match(country).Success: return "IO";
                case var country when new Regex(@"Iraque", RegexOptions.IgnoreCase).Match(country).Success: return "IQ";
                case var country when new Regex(@"Irã (Republic Islâmica do Irã)", RegexOptions.IgnoreCase).Match(country).Success: return "IR";
                case var country when new Regex(@"Islândia", RegexOptions.IgnoreCase).Match(country).Success: return "IS";
                case var country when new Regex(@"Itália", RegexOptions.IgnoreCase).Match(country).Success: return "IT";
                case var country when new Regex(@"Jersey", RegexOptions.IgnoreCase).Match(country).Success: return "JE";
                case var country when new Regex(@"Jamaica", RegexOptions.IgnoreCase).Match(country).Success: return "JM";
                case var country when new Regex(@"Jordânia", RegexOptions.IgnoreCase).Match(country).Success: return "JO";
                case var country when new Regex(@"Japão", RegexOptions.IgnoreCase).Match(country).Success: return "JP";
                case var country when new Regex(@"Quênia", RegexOptions.IgnoreCase).Match(country).Success: return "KE";
                case var country when new Regex(@"Quirguistão", RegexOptions.IgnoreCase).Match(country).Success: return "KG";
                case var country when new Regex(@"Camboja", RegexOptions.IgnoreCase).Match(country).Success: return "KH";
                case var country when new Regex(@"Kiribati", RegexOptions.IgnoreCase).Match(country).Success: return "KI";
                case var country when new Regex(@"Comores", RegexOptions.IgnoreCase).Match(country).Success: return "KM";
                case var country when new Regex(@"São Cristóvão e Nevis", RegexOptions.IgnoreCase).Match(country).Success: return "KN";
                case var country when new Regex(@"Coréia, República Popular Democrática da\)", RegexOptions.IgnoreCase).Match(country).Success: return "KP";
                case var country when new Regex(@"(Republica da)? ?Cor(é|e)ia", RegexOptions.IgnoreCase).Match(country).Success: return "KR";
                case var country when new Regex(@"Kuwait", RegexOptions.IgnoreCase).Match(country).Success: return "KW";
                case var country when new Regex(@"Ilhas Cayman", RegexOptions.IgnoreCase).Match(country).Success: return "KY";
                case var country when new Regex(@"Cazaquistão", RegexOptions.IgnoreCase).Match(country).Success: return "KZ";
                case var country when new Regex(@"República Democrática Popular do Laos", RegexOptions.IgnoreCase).Match(country).Success: return "LA";
                case var country when new Regex(@"Líbano", RegexOptions.IgnoreCase).Match(country).Success: return "LB";
                case var country when new Regex(@"Santa Lúcia", RegexOptions.IgnoreCase).Match(country).Success: return "LC";
                case var country when new Regex(@"Liechtenstein", RegexOptions.IgnoreCase).Match(country).Success: return "LI";
                case var country when new Regex(@"Sri Lanka", RegexOptions.IgnoreCase).Match(country).Success: return "LK";
                case var country when new Regex(@"Libéria", RegexOptions.IgnoreCase).Match(country).Success: return "LR";
                case var country when new Regex(@"Lesoto", RegexOptions.IgnoreCase).Match(country).Success: return "LS";
                case var country when new Regex(@"Lituânia", RegexOptions.IgnoreCase).Match(country).Success: return "LT";
                case var country when new Regex(@"Luxemburgo", RegexOptions.IgnoreCase).Match(country).Success: return "LU";
                case var country when new Regex(@"Letônia", RegexOptions.IgnoreCase).Match(country).Success: return "LV";
                case var country when new Regex(@"Líbia", RegexOptions.IgnoreCase).Match(country).Success: return "LY";
                case var country when new Regex(@"Marrocos", RegexOptions.IgnoreCase).Match(country).Success: return "MA";
                case var country when new Regex(@"Mônaco", RegexOptions.IgnoreCase).Match(country).Success: return "MC";
                case var country when new Regex(@"Moldávia, República da", RegexOptions.IgnoreCase).Match(country).Success: return "MD";
                case var country when new Regex(@"Montenegro", RegexOptions.IgnoreCase).Match(country).Success: return "ME";
                case var country when new Regex(@"São Martinho (parte francesa)", RegexOptions.IgnoreCase).Match(country).Success: return "MF";
                case var country when new Regex(@"Madagáscar", RegexOptions.IgnoreCase).Match(country).Success: return "MG";
                case var country when new Regex(@"Ilhas Marshall", RegexOptions.IgnoreCase).Match(country).Success: return "MH";
                case var country when new Regex(@"Macedônia do Norte", RegexOptions.IgnoreCase).Match(country).Success: return "MK";
                case var country when new Regex(@"Mali", RegexOptions.IgnoreCase).Match(country).Success: return "ML";
                case var country when new Regex(@"Myanmar", RegexOptions.IgnoreCase).Match(country).Success: return "MM";
                case var country when new Regex(@"Mongólia", RegexOptions.IgnoreCase).Match(country).Success: return "MN";
                case var country when new Regex(@"Macau", RegexOptions.IgnoreCase).Match(country).Success: return "MO";
                case var country when new Regex(@"Ilhas Marianas do Norte", RegexOptions.IgnoreCase).Match(country).Success: return "MP";
                case var country when new Regex(@"Martinica", RegexOptions.IgnoreCase).Match(country).Success: return "MQ";
                case var country when new Regex(@"Mauritânia", RegexOptions.IgnoreCase).Match(country).Success: return "MR";
                case var country when new Regex(@"Montserrat", RegexOptions.IgnoreCase).Match(country).Success: return "MS";
                case var country when new Regex(@"Malta", RegexOptions.IgnoreCase).Match(country).Success: return "MT";
                case var country when new Regex(@"Maurícia", RegexOptions.IgnoreCase).Match(country).Success: return "MU";
                case var country when new Regex(@"Maldivas", RegexOptions.IgnoreCase).Match(country).Success: return "MV";
                case var country when new Regex(@"Malawi", RegexOptions.IgnoreCase).Match(country).Success: return "MW";
                case var country when new Regex(@"México", RegexOptions.IgnoreCase).Match(country).Success: return "MX";
                case var country when new Regex(@"Malásia", RegexOptions.IgnoreCase).Match(country).Success: return "MY";
                case var country when new Regex(@"Moçambique", RegexOptions.IgnoreCase).Match(country).Success: return "MZ";
                case var country when new Regex(@"Namíbia", RegexOptions.IgnoreCase).Match(country).Success: return "NA";
                case var country when new Regex(@"Nova Caledônia", RegexOptions.IgnoreCase).Match(country).Success: return "NC";
                case var country when new Regex(@"Níger", RegexOptions.IgnoreCase).Match(country).Success: return "NE";
                case var country when new Regex(@"Ilha Norfolk", RegexOptions.IgnoreCase).Match(country).Success: return "NF";
                case var country when new Regex(@"Nigéria", RegexOptions.IgnoreCase).Match(country).Success: return "NG";
                case var country when new Regex(@"Nicarágua", RegexOptions.IgnoreCase).Match(country).Success: return "NI";
                case var country when new Regex(@"(Países Baixos)|(Holanda)", RegexOptions.IgnoreCase).Match(country).Success: return "NL";
                case var country when new Regex(@"Noruega", RegexOptions.IgnoreCase).Match(country).Success: return "NO";
                case var country when new Regex(@"Nepal", RegexOptions.IgnoreCase).Match(country).Success: return "NP";
                case var country when new Regex(@"Nauru", RegexOptions.IgnoreCase).Match(country).Success: return "NR";
                case var country when new Regex(@"Niue", RegexOptions.IgnoreCase).Match(country).Success: return "NU";
                case var country when new Regex(@"Nova Zelândia", RegexOptions.IgnoreCase).Match(country).Success: return "NZ";
                case var country when new Regex(@"Omã", RegexOptions.IgnoreCase).Match(country).Success: return "OM";
                case var country when new Regex(@"Panamá", RegexOptions.IgnoreCase).Match(country).Success: return "PA";
                case var country when new Regex(@"Peru", RegexOptions.IgnoreCase).Match(country).Success: return "PE";
                case var country when new Regex(@"Polinésia Francesa", RegexOptions.IgnoreCase).Match(country).Success: return "PF";
                case var country when new Regex(@"Papua Nova Guiné", RegexOptions.IgnoreCase).Match(country).Success: return "PG";
                case var country when new Regex(@"Filipinas", RegexOptions.IgnoreCase).Match(country).Success: return "PH";
                case var country when new Regex(@"Paquistão", RegexOptions.IgnoreCase).Match(country).Success: return "PK";
                case var country when new Regex(@"Polônia", RegexOptions.IgnoreCase).Match(country).Success: return "PL";
                case var country when new Regex(@"São Pedro e Miquelon", RegexOptions.IgnoreCase).Match(country).Success: return "PM";
                case var country when new Regex(@"Pitcairn", RegexOptions.IgnoreCase).Match(country).Success: return "PN";
                case var country when new Regex(@"Porto Rico", RegexOptions.IgnoreCase).Match(country).Success: return "PR";
                case var country when new Regex(@"Palestina, Estado de", RegexOptions.IgnoreCase).Match(country).Success: return "PS";
                case var country when new Regex(@"Portugal", RegexOptions.IgnoreCase).Match(country).Success: return "PT";
                case var country when new Regex(@"Palau", RegexOptions.IgnoreCase).Match(country).Success: return "PW";
                case var country when new Regex(@"Paraguai", RegexOptions.IgnoreCase).Match(country).Success: return "PY";
                case var country when new Regex(@"Catar", RegexOptions.IgnoreCase).Match(country).Success: return "QA";
                case var country when new Regex(@"Reunião", RegexOptions.IgnoreCase).Match(country).Success: return "RE";
                case var country when new Regex(@"Romênia", RegexOptions.IgnoreCase).Match(country).Success: return "RO";
                case var country when new Regex(@"Sérvia", RegexOptions.IgnoreCase).Match(country).Success: return "RS";
                case var country when new Regex(@"Federação Russa", RegexOptions.IgnoreCase).Match(country).Success: return "RU";
                case var country when new Regex(@"Ruanda", RegexOptions.IgnoreCase).Match(country).Success: return "RW";
                case var country when new Regex(@"Arábia Saudita", RegexOptions.IgnoreCase).Match(country).Success: return "SA";
                case var country when new Regex(@"Ilhas Salomão", RegexOptions.IgnoreCase).Match(country).Success: return "SB";
                case var country when new Regex(@"Seychelles", RegexOptions.IgnoreCase).Match(country).Success: return "SC";
                case var country when new Regex(@"Sudão", RegexOptions.IgnoreCase).Match(country).Success: return "SD";
                case var country when new Regex(@"Suécia", RegexOptions.IgnoreCase).Match(country).Success: return "SE";
                case var country when new Regex(@"Cingapura", RegexOptions.IgnoreCase).Match(country).Success: return "SG";
                case var country when new Regex(@"Santa Helena, Ascensão e Tristão da Cunha", RegexOptions.IgnoreCase).Match(country).Success: return "SH";
                case var country when new Regex(@"Eslovênia", RegexOptions.IgnoreCase).Match(country).Success: return "SI";
                case var country when new Regex(@"Svalbard e Jan Mayen", RegexOptions.IgnoreCase).Match(country).Success: return "SJ";
                case var country when new Regex(@"Eslováquia", RegexOptions.IgnoreCase).Match(country).Success: return "SK";
                case var country when new Regex(@"Serra Leoa", RegexOptions.IgnoreCase).Match(country).Success: return "SL";
                case var country when new Regex(@"San Marino", RegexOptions.IgnoreCase).Match(country).Success: return "SM";
                case var country when new Regex(@"Senegal", RegexOptions.IgnoreCase).Match(country).Success: return "SN";
                case var country when new Regex(@"Somália", RegexOptions.IgnoreCase).Match(country).Success: return "SO";
                case var country when new Regex(@"Suriname", RegexOptions.IgnoreCase).Match(country).Success: return "SR";
                case var country when new Regex(@"Sudão do Sul", RegexOptions.IgnoreCase).Match(country).Success: return "SS";
                case var country when new Regex(@"São Tomé e Príncipe", RegexOptions.IgnoreCase).Match(country).Success: return "ST";
                case var country when new Regex(@"El Salvador", RegexOptions.IgnoreCase).Match(country).Success: return "SV";
                case var country when new Regex(@"São Martinho (parte holandesa)", RegexOptions.IgnoreCase).Match(country).Success: return "SX";
                case var country when new Regex(@"República Árabe da Síria", RegexOptions.IgnoreCase).Match(country).Success: return "SY";
                case var country when new Regex(@"Eswatini", RegexOptions.IgnoreCase).Match(country).Success: return "SZ";
                case var country when new Regex(@"Ilhas Turks e Caicos", RegexOptions.IgnoreCase).Match(country).Success: return "TC";
                case var country when new Regex(@"Chade", RegexOptions.IgnoreCase).Match(country).Success: return "TD";
                case var country when new Regex(@"Territórios Franceses do Sul", RegexOptions.IgnoreCase).Match(country).Success: return "TF";
                case var country when new Regex(@"Ir", RegexOptions.IgnoreCase).Match(country).Success: return "TG";
                case var country when new Regex(@"Tailândia", RegexOptions.IgnoreCase).Match(country).Success: return "TH";
                case var country when new Regex(@"Tajiquistão", RegexOptions.IgnoreCase).Match(country).Success: return "TJ";
                case var country when new Regex(@"Tokelau", RegexOptions.IgnoreCase).Match(country).Success: return "TK";
                case var country when new Regex(@"Timor-Leste", RegexOptions.IgnoreCase).Match(country).Success: return "TL";
                case var country when new Regex(@"Turquemenistão", RegexOptions.IgnoreCase).Match(country).Success: return "TM";
                case var country when new Regex(@"Tunísia", RegexOptions.IgnoreCase).Match(country).Success: return "TN";
                case var country when new Regex(@"Tonga", RegexOptions.IgnoreCase).Match(country).Success: return "TO";
                case var country when new Regex(@"(Peru)|(Turquia)", RegexOptions.IgnoreCase).Match(country).Success: return "TR";
                case var country when new Regex(@"Trindade e Tobago", RegexOptions.IgnoreCase).Match(country).Success: return "TT";
                case var country when new Regex(@"Tuvalu", RegexOptions.IgnoreCase).Match(country).Success: return "TV";
                case var country when new Regex(@"Taiwan, província da China", RegexOptions.IgnoreCase).Match(country).Success: return "TW";
                case var country when new Regex(@"Tanzânia, República Unida da", RegexOptions.IgnoreCase).Match(country).Success: return "TZ";
                case var country when new Regex(@"Ucrânia", RegexOptions.IgnoreCase).Match(country).Success: return "UA";
                case var country when new Regex(@"Uganda", RegexOptions.IgnoreCase).Match(country).Success: return "UG";
                case var country when new Regex(@"Ilhas Menores Distantes dos Estados Unidos", RegexOptions.IgnoreCase).Match(country).Success: return "UM";
                case var country when new Regex(@"Estados Unidos da América", RegexOptions.IgnoreCase).Match(country).Success: return "US";
                case var country when new Regex(@"Uruguai", RegexOptions.IgnoreCase).Match(country).Success: return "UY";
                case var country when new Regex(@"Usbequistão", RegexOptions.IgnoreCase).Match(country).Success: return "UZ";
                case var country when new Regex(@"Santa Sé", RegexOptions.IgnoreCase).Match(country).Success: return "VA";
                case var country when new Regex(@"São Vicente e Granadinas", RegexOptions.IgnoreCase).Match(country).Success: return "VC";
                case var country when new Regex(@"Venezuela (República Bolivariana da)", RegexOptions.IgnoreCase).Match(country).Success: return "VE";
                case var country when new Regex(@"Ilhas Virgens Britânicas", RegexOptions.IgnoreCase).Match(country).Success: return "VG";
                case var country when new Regex(@"Ilhas Virgens (EUA)", RegexOptions.IgnoreCase).Match(country).Success: return "VI";
                case var country when new Regex(@"Vietnã", RegexOptions.IgnoreCase).Match(country).Success: return "VN";
                case var country when new Regex(@"Vanuatu", RegexOptions.IgnoreCase).Match(country).Success: return "VU";
                case var country when new Regex(@"Wallis e Futunac", RegexOptions.IgnoreCase).Match(country).Success: return "WF";
                case var country when new Regex(@"Samoa", RegexOptions.IgnoreCase).Match(country).Success: return "WS";
                case var country when new Regex(@"Iémen", RegexOptions.IgnoreCase).Match(country).Success: return "YE";
                case var country when new Regex(@"Mayotte", RegexOptions.IgnoreCase).Match(country).Success: return "YT";
                case var country when new Regex(@"(República da)? ?África do Sul", RegexOptions.IgnoreCase).Match(country).Success: return "ZA";
                case var country when new Regex(@"Zâmbia", RegexOptions.IgnoreCase).Match(country).Success: return "ZM";
                case var country when new Regex(@"Zimbábue", RegexOptions.IgnoreCase).Match(country).Success: return "ZW";
                case var country when new Regex(@"União Europ(é|e)ia", RegexOptions.IgnoreCase).Match(country).Success: return "EM";
                case var country when new Regex(@"Reino Unido", RegexOptions.IgnoreCase).Match(country).Success: return "UK";
            }
            return "ID";
        }
        public struct Owner
        {
            public string Name { get; set; }
            public string Address { get; set; }
            public string Country { get; set; }
        }
        public static Owner Process71Inid(string v)
        {
            var owner = new Owner();
            if (v.Contains("\n") && v.Contains(","))
            {
                owner.Name = v.Remove(v.IndexOf("\n")).Trim();
                owner.Address = v?.Substring(v.IndexOf("\n")).Trim();
                owner.Country = GetCountryCode(owner.Address?.Substring(owner.Address.LastIndexOf(",")).Trim());
            }
            else
                owner.Name = v;
            return owner;
        }

        /*Date*/
        public static string DateNormalize(string tmpDate)
        {
            var pattern = new Regex(@"(?<Day>\d+)\/(?<Month>\d+)\/(?<Year>\d{4})");
            var patternNewFormat = new Regex(@"(?<Day>\d+)\/(?<Month>\d+)-(?<Year>\d{4})");
            var patternFormatWithWord = new Regex(@"(?<Number>.*)\s|-(?<Date>\b\d+-[A-Z]{3}-\d+\b)\s|-(?<Country>.*)");
            var match = pattern.Match(tmpDate);
            var matchNewFormat = patternNewFormat.Match(tmpDate);
            var matchNewFormatWithWord = patternFormatWithWord.Match(tmpDate);
            if (match.Success)
            {
                return match.Groups["Year"].Value + "." + match.Groups["Month"].Value + "." + match.Groups["Day"].Value;
            }
            else if (matchNewFormat.Success)
            {
                return matchNewFormat.Groups["Year"].Value + "." + matchNewFormat.Groups["Month"].Value + "." + matchNewFormat.Groups["Day"].Value;
            }
            else if (matchNewFormatWithWord.Success)
            {
                return PrioDateNormalize(tmpDate);
            }
            else
            {
                Console.WriteLine($"Date pattern doesn't match for: ${tmpDate}");
            }
            return tmpDate;
        }

        /*Splitting records by INID numbers*/
        public static string[] RecSplit(string recString)
        {
            recString = recString
                .Replace("(71)", "")
                .Replace("(72)", "")
                .Replace("(74)", "");
                //.Replace("Nama dan Alamat yang Mengajukan Permohonan Paten :", I71)
                //.Replace("Nama Inventor :", I72)
                //.Replace("Nama dan Alamat Konsultan Paten :", I74);
            recString = Regex.Replace(recString, "Nama dan Alamat yang Mengajukan Permohonan Paten :", I71, RegexOptions.IgnoreCase);
            recString = Regex.Replace(recString, "Nama Inventor :", I72, RegexOptions.IgnoreCase);
            recString = Regex.Replace(recString, "Nama dan Alamat Konsultan Paten:", I74, RegexOptions.IgnoreCase);
            recString = Regex.Replace(recString, "Nama dan Alamat Konsultan Paten :", I74, RegexOptions.IgnoreCase);
            var I57 = @"\(57\)\s*Abstrak\s*\:*";
            var I31 = "(31) Nomor";
            var I32 = "(32) Tanggal";
            var I33 = "(33) Negara";
            string[] splittedRecord = null;
            var tempStrC = recString.Replace(I31, "").Replace(I32, "").Replace(I33, "").Trim();
            string tmpDescValue = null;
            if (tempStrC != "")
            {
                if (Regex.IsMatch(tempStrC, I57))
                {
                    tmpDescValue = tempStrC.Substring(tempStrC.IndexOf(Regex.Match(tempStrC, I57).Value)).Trim();
                    tempStrC = tempStrC.Remove(tempStrC.IndexOf(Regex.Match(tempStrC, I57).Value)).Trim();
                }
                var regexPatOne = new Regex(@"\(\d{2}\)", RegexOptions.IgnoreCase);
                var matchesClass = regexPatOne.Matches(tempStrC);
                if (matchesClass.Count > 0)
                {
                    foreach (Match matchC in matchesClass)
                    {
                        tempStrC = tempStrC.Replace(matchC.Value, "***" + matchC.Value);
                    }
                }
                /*Splitting record*/
                splittedRecord = tempStrC.Split(new string[] { "***" }, StringSplitOptions.RemoveEmptyEntries);
            }
            if (tmpDescValue != null)
            {
                splittedRecord = (splittedRecord ?? Enumerable.Empty<string>()).Concat(new string[] { tmpDescValue }).ToArray();
            }
            return splittedRecord;
        }

        public class PriorityStruct
        {
            public string Number { get; set; }
            public string Date { get; set; }
            public string Country { get; set; }
        }

        public class ClassificationInfoStruct
        {
            public string[] Class { get; set; }
            public string[] Date { get; set; }
            public string NotesValue { get; set; }
        }

        /*Classification Information 51 inid*/
        public static ClassificationInfoStruct ClassificationInfoSplit(string tmpString)
        {
            var classInfo = new ClassificationInfoStruct();
            var patternClass = @"[A-Z]{1}\d{2}[A-Z]{1}(\d+)\/(\d+)";
            var patternDate = @"\(\d{4}\.\d{2}\)";
            var I51 = @"(51) I.P.C : Int.Cl.";
            string[] splittedRecords = null;
            var tmpValue = tmpString.Replace(I51, "").Replace("\n", "").Trim();
            /*Value after // going to Notes*/
            if (tmpValue.Contains("//"))
            {
                classInfo.NotesValue = tmpValue.Substring(tmpValue.IndexOf("//")).Replace("//", "").Trim().Trim('(').Trim(')').Trim();
                tmpValue = tmpValue.Remove(tmpValue.IndexOf("//")).Trim();
            }
            /*Split records if more than one present and separated with comma*/
            if (tmpValue.Contains(","))
            {
                splittedRecords = tmpValue.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Replace(" ", "").Trim()).ToArray();
            }
            else
            {
                splittedRecords = (splittedRecords ?? Enumerable.Empty<string>()).Concat(new string[] { tmpValue.Replace(" ", "").Trim() }).ToArray();
            }
            /*Split class and date info for each record*/
            if (splittedRecords != null)
            {
                foreach (var rec in splittedRecords)
                {
                    string dateValue = null;
                    string classValue = null;
                    if (Regex.IsMatch(rec, patternClass)) { classValue = Regex.Match(rec, patternClass).Value; }
                    if (Regex.IsMatch(rec, patternDate)) { dateValue = Regex.Match(rec, patternDate).Value.Replace("(", "").Replace(")", ""); }
                    if (classValue != null)
                    {
                        classInfo.Class = (classInfo.Class ?? Enumerable.Empty<string>()).Concat(new string[] { classValue.Insert(4, " ") }).ToArray();
                    }
                    if (dateValue != null)
                    {
                        classInfo.Date = (classInfo.Date ?? Enumerable.Empty<string>()).Concat(new string[] { dateValue }).ToArray();
                    }
                }
            }
            if (classInfo != null) return classInfo;
            else return null;
        }

        /*Inventor splitting method*/
        public class InventorStruct
        {
            public string[] Name { get; set; }
            public string[] Country { get; set; }
        }
        public static InventorStruct InventorSplit(string invString)
        {
            var inventors = new InventorStruct();
            var I72 = @"(72) Nama Inventor :";
            var patternCountry = @"\,\s*[A-Z]{2}$";
            string[] splInventors = null;
            var tmpValue = invString.Replace(I72, "").Trim();
            if (tmpValue.Contains("\n"))
            {
                splInventors = tmpValue.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            }
            else
            {
                splInventors = (splInventors ?? Enumerable.Empty<string>()).Concat(new string[] { tmpValue }).ToArray();
            }
            foreach (var rec in splInventors)
            {
                string countryValue = null;
                string nameValue = null;
                if (Regex.IsMatch(rec, patternCountry))
                {
                    countryValue = Regex.Match(rec, patternCountry).Value;
                    nameValue = rec.Replace(countryValue, "").Trim();
                }
                if (countryValue != null && nameValue != null)
                {
                    inventors.Country = (inventors.Country ?? Enumerable.Empty<string>()).Concat(new string[] { countryValue.Replace(",", "").Trim() }).ToArray();
                    inventors.Name = (inventors.Name ?? Enumerable.Empty<string>()).Concat(new string[] { nameValue }).ToArray();
                }
            }

            if (inventors != null) return inventors;
            else return null;
        }
        public static string PrioDateNormalize(string tmpDate)
        {
            var swapDate = tmpDate;
            string[] splitDate = null;
            string month = null;
            if (tmpDate.Contains("-"))
            {
                try
                {
                    splitDate = tmpDate.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                    if (splitDate.Count() == 3)
                    {
                        switch (splitDate[1].ToUpper())
                        {
                            case "JAN":
                                month = "01";
                                break;
                            case "FEB":
                                month = "02";
                                break;
                            case "MAR":
                                month = "03";
                                break;
                            case "APR":
                                month = "04";
                                break;
                            case "MAY":
                                month = "05";
                                break;
                            case "JUN":
                                month = "06";
                                break;
                            case "JUL":
                                month = "07";
                                break;
                            case "AUG":
                                month = "08";
                                break;
                            case "SEP":
                                month = "09";
                                break;
                            case "OCT":
                                month = "10";
                                break;
                            case "NOV":
                                month = "11";
                                break;
                            case "DEC":
                                month = "12";
                                break;
                            default:
                                month = "00";
                                break;
                        }
                    }
                    var year = splitDate[2];
                    if (year.Length == 2)
                    {
                        year = "20" + year;
                    }
                    return swapDate = year + "-" + month + "-" + splitDate[0];
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Priority date error: {e.Message}");
                    return tmpDate;
                }
            }
            else
            {

            }
            return tmpDate;
        }
        public static PriorityStruct PrioritySplitNewFormat(string prioString)
        {
            var priorities = new PriorityStruct();
            var pattern = new Regex(@"(?<Number>.*)\s(?<Date>\b\d+-[A-Z]{3}-\d+\b)\s(?<Country>.*)");
            var tmpPrio = prioString
                .Replace("(30) Data Prioritas :", "")
                .Replace("Prioritas", "").Trim();

            if (tmpPrio != "")
            {
                var match = pattern.Match(tmpPrio);
                if (match.Success)
                {
                    priorities.Number = match.Groups["Number"].Value.Trim();
                    priorities.Date = PrioDateNormalize(match.Groups["Date"].Value.Trim());
                    priorities.Country = GetCountryCode(match.Groups["Country"].Value.Trim());
                }
                else
                {
                    Console.WriteLine($"Eror during Priority (30x) processing: value {prioString} doesn't match pattern");
                }
            }
            return priorities;
        }
        /*Sending data into Diamond*/
        public static void SendToDiamond(List<Diamond.Core.Models.LegalStatusEvent> events)
        {
            foreach (var rec in events)
            {
                var tmpValue = JsonConvert.SerializeObject(rec);
                //string url = @"https://staging.diamond.lighthouseip.online/external-api/import/legal-event"; // STAGING
                var url = @"https://diamond.lighthouseip.online/external-api/import/legal-event"; // PRODUCTION
                var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(url);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var content = new StringContent(tmpValue.ToString(), Encoding.UTF8, "application/json");
                var result = httpClient.PostAsync("", content).Result;
                var answer = result.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
