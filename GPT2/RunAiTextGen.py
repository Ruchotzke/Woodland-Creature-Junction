from aitextgen import aitextgen
import sys
import os
# Without any parameters, aitextgen() will download, cache, and load the 124M GPT-2 "small" model
ai = aitextgen()

#ai.generate()
#ai.generate(n=3, max_length=100)
#ai.generate(n=1, prompt="Player: " + sys.argv[1] + " " + sys.argv[2], max_length=70, temperature=1.2, length_penalty=1.2)
ai.generate(n=1, prompt=("Player says " + sys.argv[1] + ". Villager says "), max_length=50, temperature=1.2, length_penalty=1.2)
#ai.generate_to_file(n=10, prompt="I believe in unicorns because", max_length=100, temperature=1.2)