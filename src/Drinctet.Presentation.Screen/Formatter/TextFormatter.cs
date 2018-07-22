using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drinctet.Core;
using Drinctet.Core.Cards.Base;
using Drinctet.Core.Fragments;
using Drinctet.Core.Selection;

namespace Drinctet.Presentation.Screen.Formatter
{
    public class TextFormatter : ITextFormatter
    {
        private readonly ISelectionAlgorithm _selection;
        private readonly ITextResource _textResource;

        public TextFormatter(ISelectionAlgorithm selection, ITextResource textResource)
        {
            _selection = selection;
            _textResource = textResource;
        }

        public bool BoldPlayerNames { get; set; }
        public bool BoldSips { get; set; }

        public StringBuilder Format(IReadOnlyList<TextFragment> fragments, IReadOnlyList<PlayerSettings> playerSettings, IReadOnlyList<CardTag> tags)
        {
            var requiredPlayers = new List<PlayerSettings>();
            foreach (var fragment in fragments.OfType<PlayerReferenceFragment>().GroupBy(x => x.PlayerIndex).Select(x => x.First()))
            {
                var requiredGender = fragment.RequiredGender;
                if (requiredGender == RequiredGender.None)
                {
                    var rootSettings = playerSettings?.FirstOrDefault(x => x.PlayerIndex == fragment.PlayerIndex);
                    if (rootSettings != null)
                        requiredGender = rootSettings.Gender;
                }
                
                requiredPlayers.Add(new PlayerSettings(fragment.PlayerIndex, requiredGender));
            }

            var counter = 0;
            var players = _selection.SelectPlayers(requiredPlayers.Select(x => x.Gender).ToList(), tags)
                .ToDictionary(x => requiredPlayers[counter++].PlayerIndex, x => x);

            var sips = fragments.OfType<SipsFragment>().GroupBy(x => x.SipsIndex)
                .ToDictionary(x => x.Key, x => _selection.GetSips(x.First().MinSips));

            var builder = new StringBuilder();
            AppendFragments(builder, fragments, players, sips);
            return builder;
        }

        private void AppendFragments(StringBuilder builder, IReadOnlyList<TextFragment> fragments, IReadOnlyDictionary<int, PlayerInfo> players, IReadOnlyDictionary<int, int> sips)
        {
            PlayerReferenceFragment lastPlayerReferenceFragment = null;

            foreach (var fragment in fragments)
            {
                if (fragment is RawTextFragment rawTextFragment)
                    builder.Append(rawTextFragment.Text);
                else if (fragment is PlayerReferenceFragment playerReferenceFragment)
                {
                    if (BoldPlayerNames)
                        builder.Append("*");
                    builder.Append(players[playerReferenceFragment.PlayerIndex].Name);
                    if (BoldPlayerNames)
                        builder.Append("*");

                    lastPlayerReferenceFragment = playerReferenceFragment;
                }
                else if (fragment is SipsFragment sipsFragment)
                {
                    if (BoldSips)
                        builder.Append("*");

                    var sip = sips[sipsFragment.SipsIndex];
                    if (sip == 1)
                        builder.Append(_textResource["OneSip"]);
                    else builder.AppendFormat(_textResource["Sips"], sip);

                    if (BoldSips)
                        builder.Append("*");
                }
                else if (fragment is GenderBasedSelectionFragment genderBasedSelectionFragment)
                {
                    var referencedPlayer = genderBasedSelectionFragment.ReferencedPlayerIndex ??
                                           lastPlayerReferenceFragment?.PlayerIndex ?? players.First().Key;

                    var player = players[referencedPlayer];
                    builder.Append(player.Gender == Gender.Female
                        ? genderBasedSelectionFragment.FemaleText
                        : genderBasedSelectionFragment.MaleText);
                }
                else if (fragment is RandomTextFragment randomTextFragment)
                {
                    var index = _selection.GetRandomNumber(randomTextFragment.Texts.Count);
                    builder.Append(randomTextFragment.Texts[index]);
                }
                else if (fragment is RandomNumberFragment randomNumberFragment)
                {
                    builder.Append(_selection.SelectRandomNumber(randomNumberFragment.Numbers));
                }
                else throw new ArgumentException($"The fragment of type {fragment.GetType().Name} is not supported.");
            }
        }
    }
}