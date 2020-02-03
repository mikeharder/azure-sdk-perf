import asyncio
import logging
import os

from azure.test.perfstress import PerfStressRunner

if __name__ == '__main__':  
    main_loop = PerfStressRunner(os.path.dirname(__file__))
    loop = asyncio.get_event_loop()
    loop.run_until_complete(main_loop.RunAsync())
