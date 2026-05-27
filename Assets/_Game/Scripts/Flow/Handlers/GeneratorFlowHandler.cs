namespace App.Flow.Handlers
{
    using App.Flow.Events;
    using App.Generators.Core;
    using VitalRouter;

    [Routes]
    public partial class GeneratorFlowHandler
    {
        private readonly GeneratorService _generatorService;

        public GeneratorFlowHandler(GeneratorService generatorService)
        {
            _generatorService = generatorService;
        }

        [Route]
        void On(GenerateFromGeneratorEvent command)
        {
            _generatorService.TryGenerate(command.GeneratorId);
        }
    }
}