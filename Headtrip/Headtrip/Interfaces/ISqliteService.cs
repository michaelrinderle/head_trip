﻿/*
          __ _/| _/. _  ._/__ /
        _\/_// /_///_// / /_|/
                   _/
        copyright (c) sof digital 2021
        written by michael rinderle <michael@sofdigital.net>
*/

namespace Headtrip.Interfaces
{
    public interface ISqliteService
    {
        void ExportDb();

        string GetDbPath();
    }
}
