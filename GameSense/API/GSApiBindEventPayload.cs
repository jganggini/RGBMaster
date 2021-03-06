﻿using GameSense.API.Handlers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameSense.API
{
    public class GSApiBindEventPayload
    {
        [JsonProperty("game")]
        public string Game { get; set; }

        [JsonProperty("event")]
        public string Event { get; set; }

        [JsonProperty("min_value")]
        public int? MinValue { get; set; }

        [JsonProperty("max_value")]
        public int? MaxValue { get; set; }

        [JsonProperty("icon_id")]
        public int? IconID { get; set; }

        [JsonProperty("handlers")]
        public List<GSApiHandler> Handlers { get; set; }
    }
}
