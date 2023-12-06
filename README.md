# Circular Separable Convolutional Lens Blur (CocLb) Project Overview 
## Proposal Summary 
The project proposed the implementation of Circular Separable Convolutional Depth of Field (CocDof), a post-processing technique originating from Electronic Arts. This technique addresses the challenge of rendering bokeh depth of field efficiently, particularly when dealing with a large Circle of Confusion. Traditional methods for handling a very large Circle of Confusion are computationally expensive. The plan was to implement this algorithm using GPU shaders in Unity's real-time rendering engine, leveraging HLSL and C#. The intention was to integrate it as a post-processing effect within a practical program, capitalizing on the parallel processing capabilities of GPU architecture.

## Project Summary 
As the project unfolded, the realization dawned that implementing a full-depth-of-field system is intricate, involving considerations of various physical camera settings like aperture, focal length, and sensor size to define an appropriate mask for the plane of focus. Consequently, the project scope was refined to focus on the core blurring algorithm, opting for Lens Blur over full Depth of Field. Initial steps involved becoming acquainted with Unity's render pipeline, including the creation of render targets essential for achieving the desired effect.

## Unity Implementation  
### Gaussian Blur
The initial focus was on Gaussian Blur, chosen as the foundational element for CocLb. Both techniques require the construction of a kernel and the execution of convolutions in two separate passes. The approach involved specifying in C# where in Unity's rendering pipeline to intervene, describing the necessary passes, buffers, and uniforms for the shader, and then crafting the shader itself. This resulted in a real-time Gaussian blur with variable blur size and kernel precision. Notably, the Gaussian weights used in the kernel for convolution were dynamically calculated in the shader rather than being precomputed. Upon completion of the passes, the buffer was read into the main render target, which encompasses Unity's Viewport and Cameras.

### Circular Separable Convolutional Lens Blur
The original plan was to repurpose the Gaussian Blur as the foundation for CocLb. However, challenges arose as CocLb required additional buffers, one for each real and imaginary component to reconstruct the Circular kernel. This necessitated a single pass to generate four more passes. Unfortunately, Unity's HLSL proved to be less flexible, requiring workarounds and custom render target handlers. Attempts involving a swap chain were largely unsuccessful due to limitations in the Universal Render Pipeline. In retrospect, utilizing Unity's default renderer would have been more direct and less cumbersome. Faced with issues where the results only blurred in one axis and the second pass wouldn't bind with desired buffers, a decision was made to reimplement the algorithm in a different environment for personal satisfaction.

### MATLAB Implementation
In response to challenges encountered with Unity's URP rendering API, the decision was made to implement CocLb in MATLAB. MATLAB's efficient GPU shader dispatching facilitated a straightforward implementation. The process involved defining an input texture, a kernel, running a convolution, and combining intermediate outputs using precomputed weights as detailed in the research paper. The shift to MATLAB provided a more direct route to realizing the Circular Separable Convolutional Lens Blur algorithm, circumventing the complexities encountered in the Unity implementation.

## Conclusion
In conclusion, the project aimed to create a special blurring effect called Circular Separable Convolutional Lens Blur (CocLb) within the Unity game development environment. Originally, the goal was to achieve a realistic blur effect for in-game visuals, but due to technical challenges, the focus shifted towards a simplified version known as Lens Blur.

The Unity implementation began with the successful creation of a basic blur effect known as Gaussian Blur. This involved interrupting the standard graphics process, providing instructions for the blur, and then applying the blur horizontally and vertically. However, when attempting to enhance the effect into CocLb, Unity's tools, particularly in the Universal Render Pipeline (URP), posed difficulties in handling the required intricacies.

Recognizing these challenges, the project transitioned to MATLAB, a more straightforward tool. In MATLAB, the process became more direct – specifying how the computer should blur an image, defining rules for the blurring, and combining the blurred elements. This alternative approach proved to be more successful compared to the Unity attempt.

In summary, while Unity presented challenges, the shift to MATLAB provided a smoother pathway to achieving the desired Circular Separable Convolutional Lens Blur effect. This change showcased adaptability and problem-solving skills in the face of technical complexities.

