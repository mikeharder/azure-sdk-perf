import aiohttp

from azure.test.perfstress import PerfStressTest

class AioHttpGetTest(PerfStressTest):

    async def GlobalSetupAsync(self):
        type(self).session = aiohttp.ClientSession()

    async def GlobalCleanupAsync(self):
        await type(self).session.close()

    async def RunAsync(self):
        async with type(self).session.get(self.Arguments.url) as response:
            await response.text()

    @staticmethod
    def AddArguments(parser):
        parser.add_argument('-u', '--url', required=True)
