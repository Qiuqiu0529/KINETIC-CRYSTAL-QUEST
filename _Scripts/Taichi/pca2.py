import numpy as np
import matplotlib.pyplot as plt

def pca(data, num_components):
    # 1. 计算数据的协方差矩阵
    cov_matrix = np.cov(data.T)

    # 2. 计算协方差矩阵的特征向量和特征值
    eigenvalues, eigenvectors = np.linalg.eig(cov_matrix)

    # 3. 选择主成分
    sorted_indices = np.argsort(eigenvalues)[::-1]
    top_indices = sorted_indices[:num_components]
    top_eigenvectors = eigenvectors[:, top_indices]

    # 4. 投影到新的低维空间
    reduced_data = np.dot(data, top_eigenvectors)

    return reduced_data

# 从文件读取数据并预处理
data_file = "masterAction.txt"

with open(data_file, 'r') as f:
    lines = f.readlines()

data = []
current_matrix = []
for line in lines:
    if line.strip():  # 非空行
        values = [float(val) for val in line.split()]
        current_matrix.append(values)
    else:  # 空行，表示矩阵结束
        if current_matrix:
            data.append(np.array(current_matrix))
            current_matrix = []

if current_matrix:
    data.append(np.array(current_matrix))

data_sequence = np.array(data)

# 将多个4x3的矩阵组合成一个三维矩阵
merged_data = np.array(data_sequence)[:100]

# 调用 PCA 函数，降维为2维
reduced_data = pca(merged_data.reshape(-1, merged_data.shape[-1]), num_components=2)

# 可视化降维后的数据
plt.figure()

plt.plot(reduced_data[:, 0], reduced_data[:, 1])

plt.xlabel('Principal Component 1')
plt.ylabel('Principal Component 2')

plt.show()
