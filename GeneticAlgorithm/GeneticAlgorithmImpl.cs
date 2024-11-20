namespace GeneticAlgorithm;

/// <summary>
/// Реализация простого генетического алгоритма
/// для оптимизации функции f(x) = 7 - 45x - 63x^2 + x^3 в указанном диапазоне.
/// </summary>
public class GeneticAlgorithmImpl
{
  private readonly int _populationSize;
  private readonly int _generations;
  private readonly double _crossoverRate;
  private readonly double _mutationRate;
  private readonly double _minX;
  private readonly double _maxX;
  private readonly Random _random;

  public double[] BestChromosome { get; private set; }
  public double BestFitness { get; private set; }
  public double[] BestChromosomeMin { get; private set; }
  public double BestFitnessMin { get; private set; }

  /// <summary>
  /// Конструктор.
  /// </summary>
  /// <param name="populationSize">Количество особей в популяции</param>
  /// <param name="generations">Количество поколений для запуска алгоритма</param>
  /// <param name="crossoverRate">Вероятность кроссовера между особями</param>
  /// <param name="mutationRate">Вероятность мутации для особи</param>
  /// <param name="minX">Минимальное значение функции</param>
  /// <param name="maxX">Максимальное значение функции</param>
  public GeneticAlgorithmImpl(int populationSize, int generations, double crossoverRate, double mutationRate,
    double minX, double maxX)
  {
    _populationSize = populationSize;
    _generations = generations;
    _crossoverRate = crossoverRate;
    _mutationRate = mutationRate;
    _minX = minX;
    _maxX = maxX;
    _random = new Random();
  }

  /// <summary>
  /// Запускает генетический алгоритм.
  /// </summary>
  public void Run()
  {
    double[] population = InitializePopulation();

    for (int generation = 0; generation < _generations; generation++)
    {
      double[] fitness = EvaluateFitness(population);
      
      int bestIndex = Array.IndexOf(fitness, fitness.Max());
      if (generation == 0 || fitness[bestIndex] > BestFitness)
      {
        BestFitness = fitness[bestIndex];
        BestChromosome = new[] { population[bestIndex] };
      }
      
      int worstIndex = Array.IndexOf(fitness, fitness.Min());
      if (generation == 0 || fitness[worstIndex] < BestFitnessMin)
      {
        BestFitnessMin = fitness[worstIndex];
        BestChromosomeMin = new[] { population[worstIndex] };
      }
      
      double[] selected = Select(population, fitness);
      
      double[] offspring = Crossover(selected);
      
      Mutate(offspring);

      population = offspring;
    }
  }


  /// <summary>
  /// Инициализирует популяцию случайными значениями в указанном диапазоне.
  /// </summary>
  /// <returns>Массив случайно сгенерированных особей</returns>
  private double[] InitializePopulation()
  {
    return Enumerable.Range(0, _populationSize)
      .Select(_ => _random.NextDouble() * (_maxX - _minX) + _minX)
      .ToArray();
  }

  /// <summary>
  /// Оценивает приспособленность каждой особи в популяции.
  /// </summary>
  /// <param name="population">Массив особей в популяции</param>
  /// <returns>Массив значений приспособленности, соответствующих популяции</returns>
  private double[] EvaluateFitness(double[] population)
  {
    return population.Select(x => FitnessFunction(x)).ToArray();
  }

  /// <summary>
  /// Целевая функция f(x) = 7 - 45x - 63x^2 + x^3.
  /// </summary>
  /// <param name="x">Переменная X</param>
  /// <returns>Значение функции</returns>
  private double FitnessFunction(double x)
  {
    return 7 - 45 * x - 63 * Math.Pow(x, 2) + Math.Pow(x, 3);
  }

  /// <summary>
  /// Отбирает особей из популяции.
  /// </summary>
  /// <param name="population">Массив особей популяции</param>
  /// <param name="fitness">Массив оценок приспособленности для популяции</param>
  /// <returns>Массив выбранных особей для следующего поколения</returns>
  private double[] Select(double[] population, double[] fitness)
  {
    var totalFitness = fitness.Sum();
    return population.Select(_ =>
    {
      double pick = _random.NextDouble() * totalFitness;
      double current = 0;
      for (int i = 0; i < population.Length; i++)
      {
        current += fitness[i];
        if (current > pick)
          return population[i];
      }

      return population.Last();
    }).ToArray();
  }

  /// <summary>
  /// Осуществляет скрещивание между выбранными особями для создания потомства.
  /// </summary>
  /// <param name="population">Массив выбранных особей</param>
  /// <returns>Массив потомков, полученных в результате скрещивания</returns>
  private double[] Crossover(double[] population)
  {
    double[] offspring = new double[population.Length];
    for (int i = 0; i < population.Length; i++)
    {
      if (i + 1 < population.Length && _random.NextDouble() < _crossoverRate)
      {
        double parent1 = population[i];
        double parent2 = population[i + 1];
        double alpha = _random.NextDouble();

        // Линейный кроссовер
        offspring[i] = alpha * parent1 + (1 - alpha) * parent2;
        offspring[i + 1] = alpha * parent2 + (1 - alpha) * parent1;
      }
      else
      {
        offspring[i] = population[i];
        if (i + 1 < population.Length)
        {
          offspring[i + 1] = population[i + 1];
        }
      }
    }

    return offspring;
  }

  /// <summary>
  /// Применяет мутацию к популяции путем внесения случайных изменений.
  /// </summary>
  /// <param name="population">Массив особей популяции</param>
  private void Mutate(double[] population)
  {
    for (int i = 0; i < population.Length; i++)
    {
      if (_random.NextDouble() < _mutationRate)
      {
        // Небольшое случайное изменение
        double mutation = (_random.NextDouble() - 0.5) * (_maxX - _minX) * 0.1;
        population[i] = Math.Clamp(population[i] + mutation, _minX, _maxX);
      }
    }
  }
}