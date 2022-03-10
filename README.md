# ETEInterface
[![Build Status](https://dev.azure.com/bortgerres/ETEInterface/_apis/build/status/BorisGerretzen.ETEInterface?branchName=master)](https://dev.azure.com/bortgerres/ETEInterface/_build/latest?definitionId=4&branchName=master)
[![Build Status](https://dev.azure.com/bortgerres/ETEInterface/_apis/build/status/BorisGerretzen.ETEInterface?branchName=staging)](https://dev.azure.com/bortgerres/ETEInterface/_build/latest?definitionId=4&branchName=staging)

When my girlfriend was working on her BSc. thesis at the chair of Elastomer Technology and Engineering at the University of Twente, she had to do a lot of manual data processing. 
I made a pretty limited python script that could do this and generate graphs, but it wasn't very intuitive to use and a bit buggy overall, so I made a new version in C#.

This program converts a directory filled with .xls files *(.zs2 support coming)* from the tensile machine *(more machines coming)*, to a single .xlsx file with all of the aggregated results.
In addition to this, it is possible to import one of these aggregated .xlsx files and generate graphs from the data automatically. Take a look at the tutorial below for more information.

## How to use
![Hello World](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEYAAAAUCAAAAAAVAxSkAAABrUlEQVQ4y+3TPUvDQBgH8OdDOGa+oUMgk2MpdHIIgpSUiqC0OKirgxYX8QVFRQRpBRF8KShqLbgIYkUEteCgFVuqUEVxEIkvJFhae3m8S2KbSkcFBw9yHP88+eXucgH8kQZ/jSm4VDaIy9RKCpKac9NKgU4uEJNwhHhK3qvPBVO8rxRWmFXPF+NSM1KVMbwriAMwhDgVcrxeMZm85GR0PhvGJAAmyozJsbsxgNEir4iEjIK0SYqGd8sOR3rJAGN2BCEkOxhxMhpd8Mk0CXtZacxi1hr20mI/rzgnxayoidevcGuHXTC/q6QuYSMt1jC+gBIiMg12v2vb5NlklChiWnhmFZpwvxDGzuUzV8kOg+N8UUvNBp64vy9q3UN7gDXhwWLY2nMC3zRDibfsY7wjEkY79CdMZhrxSqqzxf4ZRPXwzWJirMicDa5KwiPeARygHXKNMQHEy3rMopDR20XNZGbJzUtrwDC/KshlLDWyqdmhxZzCsdYmf2fWZPoxCEDyfIvdtNQH0PRkH6Q51g8rFO3Qzxh2LbItcDCOpmuOsV7ntNaERe3v/lP/zO8yn4N+yNPrekmPAAAAAElFTkSuQmCC)
