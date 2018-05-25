// Copyright 2018 Esri.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific
// language governing permissions and limitations under the License.

using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using System;
using Colors = System.Drawing.Color;

namespace ArcGISRuntime.UWP.Samples.DisplayGrid
{
    [ArcGISRuntime.Samples.Shared.Attributes.Sample(
        "Display a grid",
        "MapView",
        "Display and work with coordinate grid systems such as Latitude/Longitude, MGRS, UTM and USNG on a map view. This includes toggling labels visibility, changing the color of the grid lines, and changing the color of the grid labels.",
        "Choose the grid settings and then tap 'Apply settings' to see them applied.")]
    public partial class DisplayGrid
    {
        public DisplayGrid()
        {
            InitializeComponent();

            Initialize();
        }

        private void Initialize()
        {
            // Set up the map view with a basemap.
            MyMapView.Map = new Map(Basemap.CreateImageryWithLabelsVector());

            // Configure the UI options.
            gridTypeCombo.ItemsSource = new[] { "LatLong", "MGRS", "UTM", "USNG" };
            var visibilityItemsSource = new[] { "Visible", "Invisible" };
            labelVisibilityCombo.ItemsSource = visibilityItemsSource;
            gridVisibilityCombo.ItemsSource = visibilityItemsSource;
            var colorItemsSource = new[] { "Red", "Green", "Blue", "White" };
            gridColorCombo.ItemsSource = colorItemsSource;
            labelColorCombo.ItemsSource = colorItemsSource;
            labelPositionCombo.ItemsSource = Enum.GetNames(typeof(GridLabelPosition));
            labelFormatCombo.ItemsSource = Enum.GetNames(typeof(LatitudeLongitudeGridLabelFormat));
            foreach (var combo in new[] { gridTypeCombo, labelVisibilityCombo, gridVisibilityCombo, gridColorCombo, labelColorCombo, labelPositionCombo, labelFormatCombo })
            {
                combo.SelectedIndex = 0;
            }

            // Subscribe to the button click event.
            applySettingsButton.Click += ApplySettingsButton_Click;

            // Enable the action button.
            applySettingsButton.IsEnabled = true;
        }

        private void ApplySettingsButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Grid grid;

            // First, update the grid based on the type selected.
            switch (gridTypeCombo.SelectedValue.ToString())
            {
                case "LatLong":
                    grid = new LatitudeLongitudeGrid();
                    // Apply the label format setting.
                    string selectedFormatString = labelFormatCombo.SelectedValue.ToString();
                    ((LatitudeLongitudeGrid)grid).LabelFormat =
                        (LatitudeLongitudeGridLabelFormat)Enum.Parse(typeof(LatitudeLongitudeGridLabelFormat), selectedFormatString);
                    break;

                case "MGRS":
                    grid = new MgrsGrid();
                    break;

                case "UTM":
                    grid = new UtmGrid();
                    break;

                case "USNG":
                default:
                    grid = new UsngGrid();
                    break;
            }

            // Next, apply the label visibility setting.
            switch (labelVisibilityCombo.SelectedValue.ToString())
            {
                case "Visible":
                    grid.IsLabelVisible = true;
                    break;

                case "Invisible":
                    grid.IsLabelVisible = false;
                    break;
            }

            // Next, apply the grid visibility setting.
            switch (gridVisibilityCombo.SelectedValue.ToString())
            {
                case "Visible":
                    grid.IsVisible = true;
                    break;

                case "Invisible":
                    grid.IsVisible = false;
                    break;
            }

            // Next, apply the grid color and label color settings for each zoom level.
            for (long level = 0; level < grid.LevelCount; level++)
            {
                // Set the line symbol.
                Symbol lineSymbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, Colors.FromName(gridColorCombo.SelectedItem.ToString()), 2);
                grid.SetLineSymbol(level, lineSymbol);

                // Set the text symbol.
                Symbol textSymbol = new TextSymbol
                {
                    Color = Colors.FromName(labelColorCombo.SelectedItem.ToString()),
                    OutlineColor = Colors.Purple,
                    Size = 16,
                    HaloColor = Colors.Purple,
                    HaloWidth = 3
                };
                grid.SetTextSymbol(level, textSymbol);
            }

            // Next, apply the label position setting.
            grid.LabelPosition = (GridLabelPosition)Enum.Parse(typeof(GridLabelPosition), labelPositionCombo.SelectedValue.ToString());

            // Apply the updated grid.
            MyMapView.Grid = grid;
        }
    }
}