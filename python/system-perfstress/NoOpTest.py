from azure.test.perfstress import PerfStressTest

class NoOpTest(PerfStressTest):
    def Run(self):
        pass

    async def RunAsync(self):
        pass
    