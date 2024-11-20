using System.Globalization;
using System.Windows;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace GeneticAlgorithm;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
  public PlotModel FunctionPlotModel { get; private set; }

  public MainWindow()
  {
    InitializeComponent();
    InitializeFunctionPlot();
  }

  private void InitializeFunctionPlot()
  {
    FunctionPlotModel = new PlotModel { Title = "График функции f(x)" };

    FunctionPlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "X" });
    FunctionPlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "f(x)" });

    var series = new LineSeries { Title = "f(x)", Color = OxyColors.SteelBlue };

    for (double x = -10; x <= 53; x += 0.1)
    {
      double y = FitnessFunction(x);
      series.Points.Add(new DataPoint(x, y));
    }

    FunctionPlotModel.Series.Add(series);
    FunctionPlot.Model = FunctionPlotModel;
  }

  private double FitnessFunction(double x)
  {
    return 7 - 45 * x - 63 * Math.Pow(x, 2) + Math.Pow(x, 3);
  }

  private void RunAlgorithmButton_Click(object sender, RoutedEventArgs e)
  {
    var ga = new GeneticAlgorithmImpl(
      int.Parse(PopulationSizeTextBox.Text),
      int.Parse(GenerationsTextBox.Text),
      double.Parse(CrossoverRateTextBox.Text, CultureInfo.InvariantCulture),
      double.Parse(MutationRateTextBox.Text, CultureInfo.InvariantCulture),
      -10,
      53
    );
    ga.Run();

    ResultsTextBox.AppendText($"Лучшая максимальная пригодность: {ga.BestFitness}, X: {ga.BestChromosome[0]}\n");
    ResultsTextBox.AppendText($"Лучшая минимальная пригодность: {ga.BestFitnessMin}, X: {ga.BestChromosomeMin[0]}\n");

    DisplayOptimizationResults(ga.BestChromosome[0], ga.BestChromosomeMin[0]);
  }

  private void DisplayOptimizationResults(double maxX, double minX)
  {
    var resultSeries = new ScatterSeries
    {
      Title = "Найденные экстремумы",
      MarkerType = MarkerType.Circle,
      MarkerFill = OxyColors.Red
    };

    resultSeries.Points.Add(new ScatterPoint(maxX, FitnessFunction(maxX)));
    resultSeries.Points.Add(new ScatterPoint(minX, FitnessFunction(minX)));

    FunctionPlotModel.Series.Add(resultSeries);
    FunctionPlot.InvalidatePlot();
  }
}